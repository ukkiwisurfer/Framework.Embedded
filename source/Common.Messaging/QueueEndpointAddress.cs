
namespace Ignite.Framework.Micro.Common.Messaging
{
    using NetMf.CommonExtensions;

    /// <summary>
    /// Connection details for connecting to an AMQP endpoint.
    /// </summary>
    public class QueueEndpointAddress
    {
        /// <summary>
        /// The name of the host for the AMQP server.
        /// </summary>
        public string HostName { get; set; }

        public string IPAddress { get; set; }

        /// <summary>
        /// The port address for the AQMP server.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The username to connect to the AMQP server.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The password to connect to the AMQP server.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The AMQP queue name.
        /// </summary>
        public string TargetName { get; set; }

        /// <summary>
        /// Returns a AMQP specific connection URL.
        /// </summary>
        /// <remarks>
        /// Follows the pattern: amqp://username:password@host:port
        /// </remarks>
        /// <returns>
        /// A AMQP connection string as a URL.
        /// </returns>
        public string GetUrl()
        {
            return StringUtility.Format("amqp://{0}:{1}@{2}:{3}", Username, Password, HostName, Port);
        }
    }
}
