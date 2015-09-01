namespace Ignite.Framework.Micro.Common.Contract.Messaging
{
    /// <summary>
    /// Factory for creating client connections to a message bus server.
    /// </summary>
    public interface IMessageBusFactory
    {
        /// <summary>
        /// Creates a message bus client.
        /// </summary>
        /// <returns>
        /// An instance of a message bus client.
        /// </returns>
        IMessagePublisher BuildPublisher();

        /// <summary>
        /// Creates a message bus client.
        /// </summary>
        /// <returns>
        /// An instance of a message bus client.
        /// </returns>
        IMessagePublisher BuildSubscriber();
    }
}