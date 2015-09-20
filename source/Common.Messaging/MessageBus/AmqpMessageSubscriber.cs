﻿namespace Ignite.Framework.Micro.Common.Messaging.MessageBus
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
        private ReceiverLink m_Receiver;
        private readonly IMessageHandler m_MessageHandler;
        private readonly ILogger m_Logger;
        private readonly string m_TopicName;
        private bool m_IsDisposed;
        private bool m_IsConnected;

        /// <summary>
        /// INitialises an instance of the publisher.
        /// </summary>
        /// <param name="connection">
        /// The AMQP connection to use.
        /// </param>
        /// <param name="topicName">
        /// The topic name to publish to.
        /// </param>
        /// <param name="handler">
        /// Processes incoming messages from the AMQP server.
        /// </param>
        public AmqpMessageSubscriber(AmqpConnection connection, string topicName, IMessageHandler handler)
        {
            connection.ShouldNotBeNull();
            topicName.ShouldNotBeEmpty();
            handler.ShouldNotBeNull();

            m_Connection = connection;
            m_TopicName = topicName;
            m_MessageHandler = handler;
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
        /// Starts the processing of messages sent from an AMQP server.
        /// </summary>
         /// <returns></returns>
        public void Subscribe()
        {
            m_Receiver.Start(100, OnMessage);
        }

        /// <summary>
        /// On recei[t of a message for the AMQP server, process it.
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="message"></param>
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
                m_Logger.Error("Invalid message format" ,e.CreateApplicationException("Invalid message format"));
            }
        }

        /// <summary>
        /// Returns the unique identifier of the client.
        /// </summary>
        public string ClientId
        {
            get { return m_Connection.ClientId; }
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
            if (!IsConnected)
            {
                if (!m_Connection.IsConnected) m_Connection.Connect();
                m_Receiver = new ReceiverLink(m_Connection.Session, "messages", m_TopicName);

                IsConnected = true;
            }
        }

        /// <summary>
        /// Disconnects from a AMQP server.
        /// </summary>
        public void Disconnect()
        {
            if (IsConnected)
            {
                if (m_Receiver != null)
                {
                    m_Receiver.Close();
                    m_Receiver = null;
                }

                IsConnected = false;
            }
        }
    }
}