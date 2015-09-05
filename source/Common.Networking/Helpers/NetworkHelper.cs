
namespace Ignite.Framework.Micro.Common.Networking
{  
    using Microsoft.SPOT.Net.NetworkInformation;

    /// <summary>
    /// Helper class for accessing networking details from the local device.
    /// </summary>
    public class NetworkHelper
    {
        private readonly NetworkInterface[] m_Interfaces;

        /// <summary>
        /// The number of interfaces this device supports.
        /// </summary>
        public int Count
        {
            get { return m_Interfaces.Length; }
        }

        /// <summary>
        /// Initialises an instance of the <see cref="NetworkHelper"/> class.
        /// </summary>
        public NetworkHelper()
        {
            m_Interfaces = NetworkInterface.GetAllNetworkInterfaces();            
        }

        /// <summary>
        /// Returns the network interface for a given index.
        /// </summary>
        /// <param name="interfaceIndex"></param>
        /// <returns></returns>
        private NetworkInterface GetInterface(int interfaceIndex)
        {
            if ((interfaceIndex >= 0) && (interfaceIndex < m_Interfaces.Length))
            {
                return m_Interfaces[interfaceIndex];
            }

            return null;
        }

        /// <summary>
        /// Returns the IP address of the device.
        /// </summary>
        /// <param name="interfaceIndex"></param>
        /// <returns></returns>
        public NetworkInformation GetNetworkDetails(int interfaceIndex)
        {
            var information = new NetworkInformation();

            var networkInterface = GetInterface(interfaceIndex);
            if (networkInterface != null)
            {
                information.IpAddress = networkInterface.IPAddress;
                information.IsDHCPEnabled = networkInterface.IsDhcpEnabled;
                information.SubnetMask = networkInterface.SubnetMask;
                information.MacAddress = networkInterface.PhysicalAddress;
                information.NetworkInterfaceType = networkInterface.NetworkInterfaceType.ToString();
            }

            return information;
        }
    }
}
