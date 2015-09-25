
namespace Ignite.Framework.Micro.Common.Messaging.MessageBus
{
    using Ignite.Framework.Micro.Common.Contract.Messaging;

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
        public AmqpMessageSubscriber BuildAmqpPSubscriber(AmqpConnection connection, string topicName, string linkName, IMessageHandler messageHandler)
        {
            return new AmqpMessageSubscriber(connection, topicName, linkName, messageHandler);
        }
    }
}
