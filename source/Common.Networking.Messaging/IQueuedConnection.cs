namespace Ignite.Infrastructure.Micro.Common.IO.Networking.Messaging
{
    /// <summary>
    /// Provides support for connection to a server application via message queuing technology.
    /// </summary>
    public interface IQueuedConnection
    {
        string ClientId { get; }

        /// <summary>
        /// Indicates whether the connection to the AMQP server is open.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Opens the connection to the queue,
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the connection to the queue.
        /// </summary>
        void Close();

        /// <summary>
        /// Sends a message via a queue.
        /// </summary>
        /// <param name="payload"></param>
        void SendMessage(byte[] payload);
    }
}