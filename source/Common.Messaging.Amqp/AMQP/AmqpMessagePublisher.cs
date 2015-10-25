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


namespace Ignite.Framework.Micro.Common.Messaging.AMQP
{
    using System;

    using Amqp;
    using Amqp.Framing;

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Messaging;

    /// <summary>
    /// Provides a message publishing capability (to an AMQP server).
    /// </summary>
    public class AmqpMessagePublisher : IMessagePublisher, IMessageHandler, IDisposable
    {
        private readonly AmqpConnection m_Connection;
        private SenderLink m_Sender;
        private readonly string m_TopicName;
        private readonly string m_Name;
        private string m_ConnectionId;
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
        /// Initialises an instance of the publisher.
        /// </summary>
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
        public AmqpMessagePublisher(AmqpConnection connection, string topicName, string name, bool isDurable = true)
        {
            connection.ShouldNotBeNull();
            topicName.ShouldNotBeEmpty();
            name.ShouldNotBeEmpty();

            m_ConnectionId = string.Empty;
            m_Connection = connection;
            m_TopicName = topicName;
            m_Name = name;
            m_IsDurable = isDurable;
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
        public virtual void Publish(byte[] payload)
        {
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
        public virtual void Publish(byte[] payload, bool isDurable)
        {
            try
            {
                if (!IsConnected) Connect();

                if (IsConnected)
                {
                    var message = new Message();

                    message.Header = new Header();
                    message.Header.Durable = isDurable;

                    message.Properties = new Properties();
                    message.ApplicationProperties = new ApplicationProperties();
                    message.BodySection = new Data() { Binary = payload };

                    m_Sender.Send(message);
                }
                else Disconnect();
            }
            catch (AmqpException e)
            {
                Disconnect();
            }
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
                if (!IsConnected)
                {
                    if (!m_Connection.IsConnected)
                    {
                        m_Connection.Connect();
                        if (m_Connection.IsConnected)
                        {
                            m_ConnectionId = m_Connection.ConnectionId;
                            m_Sender = new SenderLink(m_Connection.Session, m_Name, m_TopicName);

                            IsConnected = true;
                        }
                    }
                    else Disconnect();
                }
            }
            catch (AmqpException e)
            {
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
                if (IsConnected)
                {
                    if (m_Sender != null)
                    {
                        m_Sender.Close();
                        m_Sender = null;
                    }
                }
            }
            catch (AmqpException e)
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
            Publish(message);
        }
    }
}
