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

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Logging;
    using Ignite.Framework.Micro.Common.Contract.Messaging;
    using Ignite.Framework.Micro.Common.Exceptions;

    /// <summary>
    /// Processes incoming messages from an AMQP server.
    /// </summary>
    public class AmqpMessageSubscriber : IMessageSubscriber, IDisposable
    {
        private readonly AmqpConnection m_Connection;
        private readonly IMessageHandler m_MessageHandler;
        private ReceiverLink m_Receiver;
        private readonly ILogger m_Logger;
        private readonly string m_TopicName;
        private readonly string m_Name;
        private string m_ConnectionId;
        private bool m_IsDisposed;
        private bool m_IsConnected;
        private int m_WindowSize;

        /// <summary>
        /// Returns the unique identifier of the connection.
        /// </summary>
        public string ConnectionId
        {
            get { return m_ConnectionId; }
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
        /// The maximum number of messages a receiver is allowed to receive from the queue concurrently. 
        /// </summary>
        public int WindowSize
        {
            get { return m_WindowSize; }
            set
            {
                m_WindowSize = value;
                if (m_IsConnected)
                {
                    m_Receiver.SetCredit(m_WindowSize);
                }
            }
        }

        /// <summary>
        /// Indicaates whether an existing valid connection should be reused when reconnecting.
        /// </summary>
        public bool ReuseExistingConnection { get; set; }


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
        /// The unique name to associate with the link used to receive messages on.
        /// </param>
        /// <param name="handler">
        /// Processes incoming messages from the AMQP server.
        /// </param>
        public AmqpMessageSubscriber(AmqpConnection connection, string topicName, string name, IMessageHandler handler, int windowSize = 20)
        {
            connection.ShouldNotBeNull();
            topicName.ShouldNotBeEmpty();
            handler.ShouldNotBeNull();
            name.ShouldNotBeEmpty();

            m_Connection = connection;
            m_ConnectionId = string.Empty;
            m_TopicName = topicName;
            m_MessageHandler = handler;
            m_WindowSize = windowSize;
            m_Name = name;
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
        /// See <see cref="IMessageSubscriber.Subscribe"/> for more details.
        /// </summary>
        public void Subscribe()
        {
            try
            {
                m_Receiver.Start(m_WindowSize , OnMessage);
            }
            catch (AmqpException e)
            {
                //m_Logger.Error("Unable to set up subscription.", e.CreateApplicationException("Invalid subscription request."));
            }
        }

        /// <summary>
        /// See <see cref="IMessageSubscriber.Unsubscribe"/> for more details.
        /// </summary>
        public void Unsubscribe()
        {
            
        }

        /// <summary>
        /// On receipt of a message from the AMQP broker, processes it.
        /// </summary>
        /// <param name="receiver">
        /// The link used to receive the incoming messages from the AMQP broker.
        /// </param>
        /// <param name="message">
        /// The message that was received.
        /// </param>
        private void OnMessage(ReceiverLink receiver, Message message)
        {
            try
            {
                var payload = message.BodySection;
                if (payload != null)
                {
                    var buffer = new ByteBuffer(1024, true);

                    payload.Decode(buffer);
                    if (buffer.Length > 0)
                    {
                        m_MessageHandler.HandleMessage(buffer.Buffer);
                    }
                }
            }
            catch (Exception e)
            {                
                //m_Logger.Error("Invalid message format" ,e.CreateApplicationException("Invalid message format"));
            }
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
                        m_ConnectionId = m_Connection.ConnectionId;
                    }
                    m_Receiver = new ReceiverLink(m_Connection.Session, m_Name, m_TopicName);

                    m_IsConnected = true;
                }
            }
            catch (AmqpException e)
            {
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
                    if (m_Receiver != null)
                    {
                        m_Receiver.Close();
                        m_Receiver = null;
                    }

                    m_IsConnected = false;
                }
            }
            catch (AmqpException e)
            {
            }
        }


    }
}