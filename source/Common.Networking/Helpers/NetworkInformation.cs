//--------------------------------------------------------------------------- 
//   Copyright 2014-2015 Igniteous Limited
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
//----------------------------------------------------------------------------- 

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
