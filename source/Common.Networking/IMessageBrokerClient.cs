namespace Ignite.Framework.Micro.Common.Networking
{
    /// <summary>
    /// Message broker client interface definition.
    /// </summary>
    public interface IMessageBrokerClient
    {
        /// <summary>
        /// Indicates whether the connection to the AMQP server is open.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Attempts to open a connection to the message broker.
        /// </summary>
        void Open();

        /// <summary>
        /// Attempts to close an existing, open connection to a message broker.
        /// </summary>
        void Close();

        /// <summary>
        /// Sends one or more data items to the message broker.
        /// </summary>
        /// <param name="dataItems">
        /// The raw data items to send.
        /// </param>
        void SendMessages(object[] dataItems);
    }
}