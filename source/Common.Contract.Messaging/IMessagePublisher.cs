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
        /// <returns>
        /// Returns the status publishing to the message bus.
        /// </returns>
        void Publish(byte[] payload);
    }
}