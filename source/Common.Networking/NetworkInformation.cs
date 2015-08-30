
namespace Ignite.Framework.Micro.Common.Networking
{
    using System.Net;
    using System.Text;

    /// <summary>
    /// Details about the network interface. 
    /// </summary>
    public class NetworkInformation
    {
        /// <summary>
        /// The index of the network interface that was retrieved.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The IP addresss associated with the network interface.
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Indicates whether DHCP is enabled for the network interface.
        /// </summary>
        public bool IsDHCPEnabled { get; set; }

        /// <summary>
        /// The IP subnet mask associated with the network interface.
        /// </summary>
        public string SubnetMask { get; set; }

        /// <summary>
        /// The MAC address associated with the network interface.
        /// </summary>
        public byte[] MacAddress { get; set; }

        /// <summary>
        /// The type of network interface.
        /// </summary>
        public string NetworkInterfaceType { get; set; }

        /// <summary>
        /// REturns a string representation of the network information details.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("IPAddress: ").Append(IpAddress).Append(", ");
            builder.Append("IsDHCPEnabled: ").Append(IsDHCPEnabled ? "true" : "false").Append(", ");
            builder.Append("SubnetMask: ").Append(SubnetMask).Append(", ");
            builder.Append("NetworkInterfaceType: ").Append(NetworkInterfaceType);
            builder.AppendLine();

            return builder.ToString();
        }
    }
}
