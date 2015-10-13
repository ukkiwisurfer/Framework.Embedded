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
    using System;
    using System.Net;
    using System.Net.Sockets;
    using Microsoft.SPOT.Hardware;
    using Microsoft.SPOT.Net.NetworkInformation;
    using Microsoft.SPOT.Time;

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

        /// <summary>
        /// Sets the local time on the device as the same as that derived from a NTP server.
        /// </summary>
        public void SetNetworkTime()
        {
            //var settings = new TimeServiceSettings();
            //settings.ForceSyncAtWakeUp = true;
            //settings.AutoDayLightSavings = true;

            //var primary = GetHostEntry("time-a.nist.gov");
            //if (primary != null)
            //{
            //    settings.PrimaryServer = primary.GetAddressBytes();
            //}


            //TimeService.Start();

            var networkTime = GetNetworkTime();
            Utility.SetLocalTime(networkTime); 
        }

        /// <summary>
        /// Retrives the time from a TNS server.
        /// </summary>
        /// <returns></returns>
        public DateTime GetNetworkTime(string hostName = "time-a.nist.gov")
        {
            DateTime serverTime = new DateTime(1900, 1, 1);
            DateTime networkDateTime = new DateTime(1900, 1, 1);

            var hostEntry = Dns.GetHostEntry(hostName);
            if (hostEntry.AddressList.Length > 0)
            {
                IPEndPoint endpoint = new IPEndPoint(hostEntry.AddressList[0],123);
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    socket.Connect(endpoint);

                    var ntpData = BuildNtpRequest();
                    socket.Send(ntpData);
                    socket.Receive(ntpData);

                    networkDateTime = BuildNetworkTime(ntpData);
                   
                    socket.Close();
                }
            }

            return networkDateTime;
        }

        public IPAddress GetHostEntry(string hostName)
        {
             var hostEntry = Dns.GetHostEntry(hostName);
            if (hostEntry.AddressList.Length > 0)
            {
                return hostEntry.AddressList[0];
            }

            return null;
        }

        /// <summary>
        /// Parses the NTP response and constructs a network time
        /// </summary>
        /// <param name="ntpData"></param>
        /// <returns></returns>
        private DateTime BuildNetworkTime(byte[] ntpData)
        {
            var datetime = new DateTime(1900, 1, 1);

            byte offsetTransmitTime = 40;
            ulong intpart = 0;
            ulong fractpart = 0;

            for (int i = 0; i <= 3; i++)
            {
                intpart = 256 * intpart + ntpData[offsetTransmitTime + i];
            }

            for (int i = 4; i <= 7; i++)
            {
                fractpart = 256 * fractpart + ntpData[offsetTransmitTime + i];
            }

            ulong milliseconds = (intpart * 1000 + (fractpart * 1000) / 0x100000000L);
            var timeSpan = TimeSpan.FromTicks((long)milliseconds * TimeSpan.TicksPerMillisecond);
            datetime += timeSpan;

            var offsetAmount = TimeZone.CurrentTimeZone.GetUtcOffset(datetime);
            return (datetime + offsetAmount);
        }

        /// <summary>
        /// Builds a NTP request.
        /// </summary>
        /// <returns></returns>
        private byte[] BuildNtpRequest()
        {
            byte[] ntpData = new byte[48];
            ntpData[0] = 0x1B;
            
            for (int index=1; index < 48; index++)
            {
                ntpData[index] = 0;
            }

            return ntpData;
        }
    }
}
