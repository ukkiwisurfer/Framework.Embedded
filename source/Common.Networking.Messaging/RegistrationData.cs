using System;

namespace Ignite.Infrastructure.Micro.Common.Messaging
{
    using Ignite.Infrastructure.Micro.Common.IO.Networking.Messaging;

    /// <summary>
    /// Describes registraton details to connect to a message queue server. 
    /// </summary>
    public class RegistrationData
    {
        /// <summary>
        /// The name of the service to associate with sent messages.
        /// </summary>
        public string ServiceName { get; private set; }

        /// <summary>
        /// The endpoint address for the server.
        /// </summary>
        public QueueEndpointAddress Address { get; private set; }

        /// <summary>
        /// Initialies an instance of the <see cref="RegistrationData"/> class.
        /// </summary>
        /// <param name="serviceName">
        /// The name of the service to associate with sent messages.
        /// </param>
        /// <param name="address">
        /// The connection details for the server hosting the queues.
        /// </param>
        public RegistrationData(string serviceName, QueueEndpointAddress address)
        {
            this.ServiceName = serviceName;
            this.Address = address;
        }
    }
}
