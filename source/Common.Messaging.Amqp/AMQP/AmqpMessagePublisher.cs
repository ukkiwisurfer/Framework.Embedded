//--------------------------------------------------------------------------- 
//   Copyright 2014-2015 Igniteous Limited
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
//----------------------------------------------------------------------------- 

using System.Threading;
using Ignite.Framework.Micro.Common.Contract.Logging;

namespace Ignite.Framework.Micro.Common.Messaging.AMQP
{
    using System;
    using System.IO;

    using Amqp;
    using Amqp.Framing;

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Messaging;

    /// <summary>
    /// Provides a message publishing capability (to an AMQP server).
    /// </summary>
    public class AmqpMessagePublisher : IMessagePublisher, IDisposable
    {
        private readonly AmqpConnection m_Connection;
        private SenderLink m_Sender;
        private readonly string m_TopicName;
        private readonly string m_Name;
        private readonly ILogger m_Logger;
        private string m_ConnectionId;
        private int m_SendTimeoutInMilliseconds;
        private bool m_IsDisposed;
        private bool m_IsConnected;
        private bool m_IsDurable;

        /// <summary>
        /// Returns the unique identifier of the connection.
        /// </summary>
        public string ConnectionId
        {
            get { return m_ConnectionId; }
        }

        /// <summary>
        /// Timeout period in milliseconds.
        /// </summary>
        public int SendTimeoutInMilliseconds
        {
            get { return m_SendTimeoutInMilliseconds; }
            set { m_SendTimeoutInMilliseconds = value; }
        }

        /// <summary>
        /// Initialises an instance of the publisher.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="connection">
        /// The AMQP connection to use.
        /// </param>
        /// <param name="topicName">
        /// The topic name to publish to.
        /// </param>
        /// <param name="name">
        /// The unique name to associate with the link used to send messages on.
        /// </param>
        /// <param name="isDurable">
        /// Indicates whether the messages should be durable (Persistent).
        /// </param>
        public AmqpMessagePublisher(ILogger logger, AmqpConnection connection, string topicName, string name, bool isDurable = true)
        {
            connection.ShouldNotBeNull();
            topicName.ShouldNotBeEmpty();
            name.ShouldNotBeEmpty();
            logger.ShouldNotBeNull();

            m_Logger = logger;
            m_ConnectionId = string.Empty;
            m_Connection = connection;
            m_TopicName = topicName;
            m_Name = name;
            m_IsDurable = isDurable;
            m_SendTimeoutInMilliseconds = 10000;
        }

        /// <summary>
        /// See <see cref="IDisposable.Dispose"/> for more details.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases any unamanaged resources.
        /// </summary>
        /// <param name="isDisposing">
        /// Indicates whether the disposal is deterministic or not.
        /// </param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (!m_IsDisposed)
            {
                if (isDisposing)
                {
                    Disconnect();
                }

                m_IsDisposed = true;
            }
        }
        
        /// <summary>
        /// Publishes a message to a topic.
        /// </summary>
        /// <remarks>
        /// By default each message will be set to honour the publisher level durability settings.
        /// </remarks>
        /// <param name="payload">
        /// The message payload to send.
        /// </param>
        public virtual void Publish(ref byte[] payload)
        {
           Publish(ref payload, m_IsDurable);
        }

        /// <summary>
        /// Publishes a message to a topic.
        /// </summary>
        /// <remarks>
        /// By default each message will be set to honour the publisher level durability settings.
        /// </remarks>
        /// <param name="payload">
        /// The message payload to send.
        /// </param>
        public virtual void Publish(MemoryStream payload)
        {
            payload.ShouldNotBeNull();

            Publish(payload, m_IsDurable);
        }

        /// <summary>
        /// Publishes a message to a topic.
        /// </summary>
        /// <param name="payload">
        /// The message payload to send.
        /// </param>
        /// <param name="isDurable">
        /// Indicates whether the message should be persisted by the underlying queue.
        /// </param>
        public virtual void Publish(MemoryStream payload, bool isDurable)
        {
            payload.ShouldNotBeNull();

            var buffer = payload.ToArray();
            Publish(ref buffer, isDurable);
        }


        /// <summary>
        /// Publishes a message to a topic.
        /// </summary>
        /// <param name="payload">
        /// The message payload to send.
        /// </param>
        /// <param name="isDurable">
        /// Indicates whether the message should be persisted by the underlying queue.
        /// </param>
        public virtual void Publish(ref byte[] payload, bool isDurable)
        {
            payload.ShouldNotBeEmpty();

            try
            {
                Connect();

                if (IsConnected)
                {
                    PublishMessage(ref payload, isDurable);
                }
                else
                {
                    m_Logger.Error("Attempt to connect for Publish() failed. Forcing a disconnect.");
                    Disconnect();
                }
            }
            catch (Exception e)
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Publishes a message via AMQP
        /// </summary>
        /// <param name="payload">
        /// The raw data to send.
        /// </param>
        /// <param name="isDurable">
        /// Indicates whether the message should be persisted by the underlying queue.
        /// </param>
        private void PublishMessage(ref byte[] payload, bool isDurable)
        {
            var message = new Message();

            message.Header = new Header();
            message.Header.Durable = isDurable;

            message.Properties = new Properties();
            message.ApplicationProperties = new ApplicationProperties();
            message.BodySection = new Data() { Binary = payload };

            m_Logger.Debug("Publishing message to AMQP broker with timeout. TimeoutPeriod: {0} millieconds.", m_SendTimeoutInMilliseconds);
            m_Sender.Send(message, m_SendTimeoutInMilliseconds);
            m_Logger.Debug("Successfully published message to AMQP broker.");
        }

        /// <summary>
        /// Indicates whether the connection to the AMQP server is established.
        /// </summary>
        public bool IsConnected
        {
            get { return m_IsConnected; }
            private set { m_IsConnected = value; }
        }

        /// <summary>
        /// Attempts to connect to a AMQP server.
        /// </summary>
        public void Connect()
        {
            try
            {
                if (!IsConnected || !m_Connection.IsConnected)
                {
                    m_Logger.Debug("Connection not open. Attempting connection to AMQP broker.");

                    m_Connection.Connect();
                    if (m_Connection.IsConnected)
                    {
                        m_Logger.Debug("Connection open. Establishing AMQP SenderLink.");

                        m_ConnectionId = m_Connection.ConnectionId;
                        m_Sender = new SenderLink(m_Connection.Session, m_Name, m_TopicName);
                        m_Logger.Debug("Connected AMQP publisher to broker. LinkName: '{0}', TopicName: '{1}'", m_Name, m_TopicName);

                        IsConnected = true;
                    }
                    else
                    {
                        m_Logger.Error("Attempt to connect failed. Forcing a disconnect.");
                        Disconnect();
                    }
                }
            }
            catch (Exception ex)
            {
                m_Logger.Fatal("Exception raised during Connect() failed. Forcing disconnect.", ex);
                Disconnect();
            }
        }
        
        /// <summary>
        /// Disconnects from a AMQP server.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (m_Sender != null)
                {
                    m_Sender.Close();
                    m_Sender = null;
                }
                
            }
            catch (Exception e)
            {
            }
            finally
            {
                IsConnected = false;                
            }
        }

        /// <summary>
        /// See <see cref="IMessageHandler.HandleMessage"/> for more details.
        /// </summary>
        /// <param name="message">
        /// The message payload to send.
        /// </param>
        public void HandleMessage(byte[] message)
        {
            Publish(ref message);
        }
    }
}
