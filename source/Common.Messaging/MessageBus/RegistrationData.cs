namespace Ignite.Framework.Micro.Common.Messaging.MessageBus
{
    /// <summary>
    /// Describes registraton details to connect to a message queue server. 
    /// </summary>
    public class RegistrationData
    {
        /// <summary>
        /// The endpoint address for the server.
        /// </summary>
        public QueueEndpointAddress Address { get; private set; }

        /// <summary>
        /// Initialies an instance of the <see cref="RegistrationData"/> class.
        /// </summary>
        /// <param name="address">
        /// The connection details for the server hosting the queues.
        /// </param>
        public RegistrationData(QueueEndpointAddress address)
        {
            this.Address = address;
        }
    }
}
