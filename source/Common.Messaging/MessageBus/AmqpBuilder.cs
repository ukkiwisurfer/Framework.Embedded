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
    using Ignite.Framework.Micro.Common.Contract.Messaging;
    using Microsoft.SPOT;

    /// <summary>
    /// Builds AMQP components.
    /// </summary>
    public class AmqpBuilder
    {

        /// <summary>
        /// Builds an AMQP connection.
        /// </summary>
        /// <returns>
        /// An initialised instance of a <see cref="AmqpConnection"/> class.
        /// </returns>
        public AmqpConnection BuildAmqpConnection(QueueEndpointAddress endpointAddress)
        {
            var configuration = new RegistrationData(endpointAddress);
            return new AmqpConnection(configuration);
        }

        /// <summary>
        /// Builds an AMQP connection.
        /// </summary>
        /// <param name="endpointAddress"></param>
        /// <param name="closedEventHandler"></param>
        /// <returns>
        /// An initialised instance of a <see cref="AmqpConnection"/> class.
        /// </returns>
        public AmqpConnection BuildAmqpConnection(QueueEndpointAddress endpointAddress, EventHandler closedEventHandler)
        {
            var configuration = new RegistrationData(endpointAddress);
            return new AmqpConnection(configuration, closedEventHandler);
        }

        /// <summary>
        /// Builds an AMQP publisher.
        /// </summary>
        /// <param name="connection">
        /// The AMQP connection instance to use for publishing messages (pub/sub).
        /// </param>
        /// <param name="topicName">
        /// The name of the AMQP topic to publish 
        /// </param>
        /// <param name="linkName">
        /// The unique name to associate with the link used to send messages on.
        /// </param>
        /// <returns>
        /// An initialised instance of a <see cref="AmqpMessagePublisher"/> class.
        /// </returns>
        public AmqpMessagePublisher BuildAmqpPublisher(AmqpConnection connection, string topicName, string linkName)
        {
            return new AmqpMessagePublisher(connection, topicName, linkName);
        }

        /// <summary>
        /// Builds an AMQP subscriber.
        /// </summary>
        /// <param name="connection">
        /// The AMQP connection instance to use for publishing messages (pub/sub).
        /// </param>
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
        public AmqpMessageSubscriber BuildAmqpSubscriber(AmqpConnection connection, string topicName, string linkName, IMessageHandler messageHandler, int windowSize = 20)
        {
            return new AmqpMessageSubscriber(connection, topicName, linkName, messageHandler, windowSize);
        }
    }
}
