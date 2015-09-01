namespace Ignite.Framework.Micro.Common.Contract.Messaging
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
        bool IsConnected { get; }

        /// <summary>
        /// Opens the connection to the queue,
        /// </summary>
        void Connect();

        /// <summary>
        /// Closes the connection to the queue.
        /// </summary>
        void Disconnect();
    }
}