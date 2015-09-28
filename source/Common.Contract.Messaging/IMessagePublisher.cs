namespace Ignite.Framework.Micro.Common.Contract.Messaging
{
    /// <summary>
    /// Provides the means to publish messages to a message bus.
    /// </summary>
    public interface IMessagePublisher : IQueuedConnection
    {
        /// <summary>
        /// Publishes a message on the message bus.
        /// </summary>
        /// <param name="payload">
        /// The payload to publish.
        /// </param>
        void Publish(byte[] payload);

        /// <summary>
        /// Publishes a message on the message bus.
        /// </summary>
        /// <param name="payload">
        /// The payload to publish.
        /// </param>
        /// <param name="isDurable">
        /// Indicates whether the message should be persisted by the queue.
        /// </param>
        void Publish(byte[] payload, bool isDurable);

    }
}