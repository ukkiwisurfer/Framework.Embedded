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

namespace Ignite.Framework.Micro.Common.Messaging.MessageBus
{
    using System;
    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Messaging;
    using Microsoft.SPOT;

    /// <summary>
    /// A proxy for an AMQP publisher.
    /// </summary>
    /// <remarks>
    /// Supports error management against an AMQP connection and publisher. Will force an
    /// AMQP connection that has closed or errored to be recreated.
    /// </remarks>
    public class AmqpSubscriberProxy : IMessageSubscriber, IDisposable
    {
        private readonly QueueEndpointAddress m_Address;
        private readonly AmqpBuilder m_Builder;
        private readonly IMessageHandler m_MessageHandler;
        private readonly string m_TopicName;
        private readonly string m_Name;
        private readonly bool m_IsDurable;
        private AmqpMessageSubscriber m_Subscriber;
        private AmqpConnection m_Connection;
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
                    m_Subscriber.WindowSize = m_WindowSize;
                }
            }
        }

        /// <summary>
        /// Indicaates whether an existing valid connection should be reused when reconnecting.
        /// </summary>
        public bool ReuseExistingConnection { get; set; }

        /// <summary>
        /// Initialises an instance of the <see cref="AmqpSubscriberProxy"/> class.
        /// </summary>
        /// <param name="builder">
        /// Builder used to create AMQP resources.
        /// </param>
        /// <param name="endpointAddress">
        /// The endpoint address for the AMQP server.
        /// </param>
        /// <param name="topicName">
        /// The topic name to publish to.
        /// </param>
        /// <param name="name">
        /// The unique name to associate with the link used to send messages on.
        /// </param>
        public AmqpSubscriberProxy(AmqpBuilder builder, QueueEndpointAddress endpointAddress, string topicName, string name, IMessageHandler handler, int windowSize = 5)
        {
            builder.ShouldNotBeNull();
            endpointAddress.ShouldNotBeNull();
            topicName.ShouldNotBeEmpty();
            name.ShouldNotBeEmpty();
            handler.ShouldNotBeNull();

            m_Builder = builder;
            m_Address = endpointAddress;
            m_MessageHandler = handler;

            m_TopicName = topicName;
            m_ConnectionId = string.Empty;
            m_Name = name;
        }

        /// <summary>
        /// See <see cref="IMessagePublisher.Connect"/> for more details.
        /// </summary>
        public void Connect()
        {
            if (!m_IsConnected)
            {
                try
                {
                    m_Connection = BuildAmqpConnection(ReuseExistingConnection);
                    m_ConnectionId = m_Connection.ConnectionId;

                    m_Subscriber = m_Builder.BuildAmqpSubscriber(m_Connection, m_TopicName, m_Name, m_MessageHandler, m_WindowSize);
                    m_IsConnected = true;

                }
                catch (Exception)
                {
                    Disconnect();
                }
            }
        }

        /// <summary>
        /// Builds an AMQP connection.
        /// </summary>
        /// <returns>
        /// An AMQP connection.
        /// </returns>
        protected virtual AmqpConnection BuildAmqpConnection(bool reuseExistingConnection = true)
        {
            AmqpConnection connection;

            if (reuseExistingConnection && ((m_Connection != null) && (m_Connection.IsConnected)))
            {
                connection = m_Connection;
            }
            else connection = m_Builder.BuildAmqpConnection(m_Address, ClosedEventHandler);

            return connection;
        }

        /// <summary>
        /// Event handler for when errors are detected or the AMQP connection closes.
        /// </summary>
        /// <param name="sender">
        /// The object associated with the event.
        /// </param>
        /// <param name="eventArgs">
        /// The event arguments associated with the close event.
        /// </param>
        private void ClosedEventHandler(object sender, EventArgs eventArgs)
        {
            Disconnect();
        }

        /// <summary>
        /// See <see cref="IMessagePublisher.Disconnect"/> for more details.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (m_Subscriber != null) m_Subscriber.Dispose();
                if (m_Connection != null) m_Connection.Dispose();

                m_Subscriber = null;
                m_Connection = null;

                m_ConnectionId = string.Empty;
            }
            finally
            {
                m_IsConnected = false;
            }
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
        public void Subscribe()
        {
            try
            {
                Connect();

                if (m_IsConnected)
                {
                    m_Subscriber.Subscribe();
                }
            }
            catch (Exception)
            {
                Disconnect();
            }
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
    }
}
