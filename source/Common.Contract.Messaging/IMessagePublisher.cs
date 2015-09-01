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
        /// <param name="topicName">
        /// The name of the topic to publish the message to.
        /// </param>
        /// <param name="payload">
        /// The payload to publish.
        /// </param>
        /// <param name="retainMessage">
        /// Indicates whether the message should be retained (persisted).
        /// </param>
        /// <returns>
        /// Returns the status publishing to the message bus.
        /// </returns>
        void Publish(byte[] payload);
    }
}