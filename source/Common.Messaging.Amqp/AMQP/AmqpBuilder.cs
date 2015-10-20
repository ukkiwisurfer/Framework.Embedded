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
    using System.Collections;

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
        private bool m_IsDisposed;

        /// <summary>
        /// Initialises an instance of the <see cref="AmqpBuilder"/> class
        /// </summary>
        private AmqpBuilder()
        {
        }

        /// <summary>
        /// Initialises an instance of the <see cref="AmqpBuilder"/> class.
        /// </summary>
        /// <param name="endpointAddress"></param>
        public AmqpBuilder(QueueEndpointAddress endpointAddress) : this()
        {
            endpointAddress.ShouldNotBeNull();

            m_Connection = BuildConnection(endpointAddress);
            m_EndpointAddress = endpointAddress;
        }

        /// <summary>
        /// Initialises an instance of the <see cref="AmqpBuilder"/> class.
        /// </summary>
        /// <param name="endpointAddress"></param>
        /// <param name="closedEventHandler"></param>
        public AmqpBuilder(QueueEndpointAddress endpointAddress, EventHandler closedEventHandler) : this()
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
            return new AmqpConnection(configuration);
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
            return new AmqpConnection(configuration, closedEventHandler);
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
            var publisher = new AmqpPublisherProxy(this, m_EndpointAddress, topicName, linkName, true);
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
            var subscriber = new AmqpMessageSubscriber(m_Connection, topicName, linkName, messageHandler, 20);
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
            var subscriber = new AmqpMessageSubscriber(m_Connection, topicName, linkName, messageHandler, windowSize);
            return subscriber;
        }
    }
}
