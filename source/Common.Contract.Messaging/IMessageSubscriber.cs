namespace Ignite.Framework.Micro.Common.Contract.Messaging
{
    public interface IMessageSubscriber : IQueuedConnection
    {
        /// <summary>
        /// Subscribes to a message topic on the message bus.
        /// </summary>
        /// <param name="topicName">
        /// The name of the topic to publish the message to.
        /// </param>
        /// <returns>
        /// Returns the status publishing to the message bus.
        /// </returns>
        void Subscribe();
    }
}