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

using Ignite.Framework.Micro.Common.Contract.Logging;

namespace Ignite.Framework.Micro.Common.Messaging.AMQP
{
    using System;

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Messaging;

    using Microsoft.SPOT;

    /// <summary>
    /// Builds AMQP components.
    /// </summary>
    public class AmqpBuilder : IMessageBrokerFactory
    {
        private readonly AmqpConnection m_Connection;
        private readonly QueueEndpointAddress m_EndpointAddress;
        private readonly ILogFactory m_LogFactory;
        private bool m_IsDisposed;

        /// <summary>
        /// Initialises an instance of the <see cref="AmqpBuilder"/> class
        /// </summary>
        /// <param name="logFactory"></param>
        private AmqpBuilder(ILogFactory logFactory)
        {
            logFactory.ShouldNotBeNull();

            m_LogFactory = logFactory;
        }

        /// <summary>
        /// Initialises an instance of the <see cref="AmqpBuilder"/> class.
        /// </summary>
        /// <param name="logFactory"></param>
        /// <param name="endpointAddress"></param>
        public AmqpBuilder(ILogFactory logFactory, QueueEndpointAddress endpointAddress) : this(logFactory)
        {
            endpointAddress.ShouldNotBeNull();

            m_Connection = BuildConnection(endpointAddress);
            m_EndpointAddress = endpointAddress;
        }

        /// <summary>
        /// Initialises an instance of the <see cref="AmqpBuilder"/> class.
        /// </summary>
        /// <param name="logFactory"></param>
        /// <param name="endpointAddress"></param>
        /// <param name="closedEventHandler"></param>
        public AmqpBuilder(ILogFactory logFactory, QueueEndpointAddress endpointAddress, EventHandler closedEventHandler) : this(logFactory)
        {
            endpointAddress.ShouldNotBeNull();

            m_Connection = BuildConnection(endpointAddress, closedEventHandler);
            m_EndpointAddress = endpointAddress;
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
                    m_Connection.Dispose();
                }

                m_IsDisposed = true;
            }
        }

        /// <summary>
        /// Releases a disposable resources.
        /// </summary>
        /// <param name="resource"></param>
        private void DisposeResource(object resource)
        {
            var disposable = resource as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// Builds an AMQP connection.
        /// </summary>
        /// <returns>
        /// An initialised instance of a <see cref="AmqpConnection"/> class.
        /// </returns>
        public AmqpConnection BuildConnection(QueueEndpointAddress endpointAddress)
        {
            var configuration = new RegistrationData(endpointAddress);
            var logger = m_LogFactory.GetLogger(typeof(AmqpConnection));

            return new AmqpConnection(logger , configuration);
        }

        /// <summary>
        /// Builds an AMQP connection.
        /// </summary>
        /// <returns>
        /// An initialised instance of a <see cref="AmqpConnection"/> class.
        /// </returns>
        public AmqpConnection BuildConnection(QueueEndpointAddress endpointAddress, EventHandler closedEventHandler)
        {
            var configuration = new RegistrationData(endpointAddress);
            var logger = m_LogFactory.GetLogger(typeof (AmqpConnection));

            return new AmqpConnection(logger, configuration, closedEventHandler);
        }

        /// <summary>
        /// Builds an AMQP publisher.
        /// </summary>
        /// <param name="topicName">
        /// The name of the AMQP topic to publish 
        /// </param>
        /// <param name="linkName">
        /// The unique name to associate with the link used to send messages on.
        /// </param>
        /// <returns>
        /// An initialised instance of a <see cref="AmqpMessagePublisher"/> class.
        /// </returns>
        public IMessagePublisher BuildPublisher(string topicName, string linkName)
        {
            //var publisher = new AmqpMessagePublisher(m_Connection, topicName, linkName);
            var connection = BuildConnection(m_EndpointAddress);
            var logger = m_LogFactory.GetLogger(typeof(AmqpMessagePublisher));
            var publisher = new AmqpMessagePublisher(logger, connection, topicName, linkName);

            return publisher;
        }

        /// <summary>
        /// Builds an AMQP subscriber.
        /// </summary>
        /// <param name="topicName">
        /// The name of the AMQP topic to subscribe to. 
        /// </param>
        /// <param name="linkName">
        /// The unique name to associate with the link used to receive messages on.
        /// </param>
        /// <param name="messageHandler">
        /// Processes any incoming message payloads.
        /// </param>
        /// <returns>
        /// An initialised instance of a <see cref="AmqpMessageSubscriber"/> class.
        /// </returns>
        public IMessageSubscriber BuildSubscriber(string topicName, string linkName, IMessageHandler messageHandler)
        {
            var logger = m_LogFactory.GetLogger(typeof(AmqpMessageSubscriber));
            var subscriber = new AmqpMessageSubscriber(logger, m_Connection, topicName, linkName, messageHandler, windowSize: 20);

            return subscriber;
        }

        /// <summary>
        /// Builds an AMQP subscriber.
        /// </summary>
        /// <param name="topicName">
        /// The name of the AMQP topic to subscribe to. 
        /// </param>
        /// <param name="linkName">
        /// The unique name to associate with the link used to receive messages on.
        /// </param>
        /// <param name="messageHandler">
        /// Processes any incoming message payloads.
        /// </param>
        /// <returns>
        /// An initialised instance of a <see cref="AmqpMessageSubscriber"/> class.
        /// </returns>
        public IMessageSubscriber BuildSubscriber(string topicName, string linkName, IMessageHandler messageHandler, int windowSize)
        {
            var logger = m_LogFactory.GetLogger(typeof(AmqpMessageSubscriber));
            var subscriber = new AmqpMessageSubscriber(logger, m_Connection, topicName, linkName, messageHandler, windowSize);

            return subscriber;
        }
    }
}
