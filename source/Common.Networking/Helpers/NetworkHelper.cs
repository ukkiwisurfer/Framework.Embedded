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

using Ignite.Framework.Micro.Common.Contract.Networking;

namespace Ignite.Framework.Micro.Common.Networking
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    using Microsoft.SPOT;
    using Microsoft.SPOT.Hardware;
    using Microsoft.SPOT.Net.NetworkInformation;

    /// <summary>
    /// Helper class for accessing networking details from the local device.
    /// </summary>
    public class NetworkHelper
    {
        private NetworkAvailabilityChangedEvent m_OnNetworkAvailabilityChanged;
        private readonly NetworkInterface[] m_Interfaces;
        private readonly AutoResetEvent m_WaitForNetwork;
        private readonly AutoResetEvent m_WaitForAddressChange;
        private readonly object m_SyncLock;
        private int m_WaitForNetworkEventInMilliseconds;
      
        /// <summary>
        /// The number of interfaces this device supports.
        /// </summary>
        public int Count
        {
            get { return m_Interfaces.Length; }
        }

        /// <summary>
        /// Specifies the period to wait in milliseconds for a network change event.
        /// </summary>
        public int WaitForNetworkEventInMilliseconds
        {
            get { return m_WaitForNetworkEventInMilliseconds; }
            set { m_WaitForNetworkEventInMilliseconds = value; }
        }

        /// <summary>
        /// Event handler for when the network availability changes.
        /// </summary>
        public event NetworkAvailabilityChangedEvent NetworkAvailabilityChanged
        {
            add
            {
                lock (m_SyncLock)
                {
                    m_OnNetworkAvailabilityChanged += value;
                }
            }
            remove
            {
                lock (m_SyncLock)
                {
                    var handler = m_OnNetworkAvailabilityChanged;
                    if (handler != null)
                    {
                        handler -= value;
                    }
                }
            }
        }

        /// <summary>
        /// Initialises an instance of the <see cref="NetworkHelper"/> class.
        /// </summary>
        public NetworkHelper()
        {
            m_Interfaces = NetworkInterface.GetAllNetworkInterfaces();
            m_WaitForNetwork = new AutoResetEvent(false);
            m_WaitForAddressChange = new AutoResetEvent(false);
            m_SyncLock = new object();
            m_WaitForNetworkEventInMilliseconds = 2000;
        }

        /// <summary>
        /// Returns the network interface for a given index.
        /// </summary>
        /// <param name="interfaceIndex">
        ///  The index of the network interface to query.
        /// </param>
        /// <returns>
        /// Details of the network interface.
        /// </returns>
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
        /// <remarks>
        /// On the Netduino Plus 2, the Ethernet is initialised way before the software starts
        /// running. On the Netduino 3 (WiFi) it takes considerably longer to initialise.
        /// <para></para>
        /// The consequence is that for this to work on both boards we need to test to see if the
        /// IP address has already been configured. otherwise the WaitHandle.WaitAll() will wait
        /// indefinitely as the event handlers are too late in terms of signalling - the network
        /// has already been configured.
        /// <para></para>
        /// This demonstrates a weakness in this approach in that we could have a race condition -
        /// the network has already been configured before the event handlers are set up. We must 
        /// therefore periodically timeout from the WaitHandle.WaitAll() and query the status
        /// of the network interface. 
        /// </remarks>
        /// <param name="interfaceIndex">
        /// The index of the network interface to query.
        /// </param>
        /// <returns>
        /// Information about the specified network interface.
        /// </returns>
        public NetworkInformation GetNetworkDetails(int interfaceIndex)
        {
            NetworkInformation information = null;

            // Set up event handlers for network state changes.
            NetworkChange.NetworkAvailabilityChanged += OnInternalNetworkAvailabilityChanged;
            NetworkChange.NetworkAddressChanged += OnNetworkAddressChanged;

            try
            {
                // Wait for the network to become available and for a DHCP address to be allocated.
                bool isWaitingToInitialise = true;
                while (isWaitingToInitialise)
                {
                    var isSignalled = WaitHandle.WaitAll(new[] { m_WaitForAddressChange, m_WaitForNetwork }, m_WaitForNetworkEventInMilliseconds, false);
                    if (!isSignalled)
                    {
                        // Retrieve network interface details to see if the initialisation had already occurred.
                        information = QueryInterface(interfaceIndex);
                        if ((information.IsDHCPEnabled) && (information.IpAddress != "0.0.0.0"))
                        {
                            isWaitingToInitialise = false;
                        }
                    }
                    else isWaitingToInitialise = false;
                }

                information = QueryInterface(interfaceIndex);
            }
            finally
            {
                m_WaitForAddressChange.Reset();
                m_WaitForNetwork.Reset();
            }

            return information;
        }

        /// <summary>
        /// Queries a network device for details about its connection state.
        /// </summary>
        /// <param name="interfaceIndex">
        /// The index of the device to query.
        /// </param>
        /// <returns>
        /// The status information on the network device.
        /// </returns>
        public NetworkInformation QueryInterface(int interfaceIndex)
        {
            NetworkInformation information = null;

            if ((interfaceIndex >= 0) && (interfaceIndex < m_Interfaces.Length))
            {
                var networkInterface = GetInterface(interfaceIndex);
                if (networkInterface != null)
                {
                    // Then capture the network details. 
                    information = new NetworkInformation();
                    information.IpAddress = networkInterface.IPAddress;
                    information.IsDHCPEnabled = networkInterface.IsDhcpEnabled;
                    information.SubnetMask = networkInterface.SubnetMask;
                    information.MacAddress = networkInterface.PhysicalAddress;
                    information.NetworkInterfaceType = networkInterface.NetworkInterfaceType.ToString();
                }
            }

            return information;
        }

        /// <summary>
        /// Event handler to signal when the network address has changed.
        /// </summary>
        /// <param name="sender">
        /// The object that initiated the event.
        /// </param>
        /// <param name="eventArgs">
        /// The event arguments associated with the change in network address.
        /// </param>
        private void OnNetworkAddressChanged(object sender, EventArgs eventArgs)
        {
            m_WaitForAddressChange.Set();
        }

        /// <summary>
        /// Event handler for indicating when the network availability changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="isNetworkAvailable">
        /// Indicates the status of the 
        /// </param>
        private void OnInternalNetworkAvailabilityChanged(object sender, bool isNetworkAvailable)
        {
            if (m_OnNetworkAvailabilityChanged != null)
            {
                m_OnNetworkAvailabilityChanged(isNetworkAvailable);
            }
        }


        /// <summary>
        /// Event handler to signal that the network is now available.
        /// </summary>
        /// <param name="sender">
        /// The object that initiated the event.
        /// </param>
        /// <param name="networkAvailabilityEventArgs">
        /// The event arguments associated with the change in network availability,
        /// </param>
        private void OnInternalNetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs networkAvailabilityEventArgs)
        {
            if (networkAvailabilityEventArgs.IsAvailable)
            {
                m_WaitForNetwork.Set();
            }

            OnInternalNetworkAvailabilityChanged(this, networkAvailabilityEventArgs.IsAvailable);
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
        /// <returns>
        /// The calculated timestamp to associate to the device.
        /// </returns>
        public DateTime GetNetworkTime(string hostName = "time-a.nist.gov")
        {
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

        /// <summary>
        /// Returns the IP address for the given host name.
        /// </summary>
        /// <param name="hostName">
        /// The name of thw host to query for.
        /// </param>
        /// <returns>
        /// The IP address of the specified host (if found) otherwise null.
        /// </returns>
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
        /// <param name="ntpData">
        /// The payload from the NTP service used to calculate the current local time. 
        /// </param>
        /// <returns>
        /// The calculated network time.
        /// </returns>
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
        /// Builds a NTP request poacket.
        /// </summary>
        /// <returns>
        /// A NTP request packet.
        /// </returns>
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
