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

    using Ignite.Framework.Micro.Common.Contract.Messaging;


    /// <summary>
    /// Represents a UDP socket.
    /// </summary>
    /// <remarks>
    /// Captures UDP packets and passes the received data to a <see cref="IMessageHandler"/> 
    /// instance for processing.
    /// </remarks>
    public class UdpSocket : IDisposable
    {
        private readonly Socket m_Client;
        private readonly IPEndPoint m_Endpoint;
        private readonly IMessageHandler m_MessageHandler;
        private readonly object m_SyncLock;
        private  bool m_IsOpen;
        private bool m_IsDisposed;

        /// <summary>
        /// Indicates if the socket is open.
        /// </summary>
        public bool IsOpen
        {
            get { return m_IsOpen; }
        }

        /// <summary>
        /// The size of the buffer to allocate in bytes.
        /// </summary>
        public int BufferSizeInBytes { get; set; }

        /// <summary>
        /// The period to wait for any incoming bytes in millseconds.
        /// </summary>
        /// <remarks>
        /// This cannot be set to a value less than 500 milliseconds. The .NET
        /// Micro framework will not honour a value less than this.
        /// </remarks>
        public int PeekTimeoutInMilliseconds { get; set; }

        /// <summary>
        /// Initialises an instance of the <see cref="UdpSocket"/> class.
        /// </summary>
        /// <param name="multicastAddress">
        /// The multicast that the Owl server is broadcasting data on.
        /// </param>
        /// <param name="port">
        /// The port that the Owl server is broadcasting data on.
        /// </param>
        /// <param name="handler">
        /// Hanlder for incoming data.
        /// </param>
        public UdpSocket(string multicastAddress, int port, IMessageHandler handler)
        {
            m_Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            m_Endpoint = new IPEndPoint(IPAddress.Parse(multicastAddress), port);
            m_MessageHandler = handler;

            m_SyncLock = new object();

            BufferSizeInBytes = 1024;
            PeekTimeoutInMilliseconds = 500;
        }

        /// <summary>
        /// See <see cref="IDisposable.Dispose"/> for more details.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases any unamanaged resources.
        /// </summary>
        /// <param name="isDisposing">
        /// Indicates whether the disposal is deterministic or not.
        /// </param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (!m_IsDisposed)
            {
                if (isDisposing)
                {
                    m_Client.Close();

                    var disposable = m_Client as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }

                m_IsDisposed = true;
            }
        }

        /// <summary>
        /// Starts the client connection.
        /// </summary>
        public void Open()
        {
            m_IsOpen = false;
            lock (m_SyncLock)
            {
                m_Client.Bind(new IPEndPoint(IPAddress.Any, m_Endpoint.Port));

                m_IsOpen = true;
            }
        }

        /// <summary>
        /// Ends the client connection.
        /// </summary>
        public void Close()
        {
            lock (m_SyncLock)
            {
                m_Client.Close();
                m_IsOpen = false;
            }
        }

        /// <summary>
        /// Checks for incoming data.
        /// </summary>
        /// <returns>
        /// True if incoming data exists and hasn't been processed.
        /// </returns>
        public bool CheckForIncomingData()
        {
            lock (m_SyncLock)
            {
                bool hasPayload = m_Client.Poll(PeekTimeoutInMilliseconds, SelectMode.SelectRead);
                return hasPayload;
            }
        }

        /// <summary>
        /// Checks to see if there is any data on the socket to process.
        /// </summary>
        /// <remarks>
        /// Will not block indefinitely while waiting for data or throwing 
        /// an exception if a timeout was specified on the read.
        /// </remarks>
        public void ListenForMessage()
        {
            EndPoint remoteendpoint = null;
            ConnectionState connectionState = null;
            int bytesRead = 0;
            
            lock (m_SyncLock)
            {
                if (m_IsOpen)
                {
                    remoteendpoint = new IPEndPoint(m_Endpoint.Address, m_Endpoint.Port);
                    connectionState = new ConnectionState(m_Client, BufferSizeInBytes);

                    bool hasPayload = m_Client.Poll(PeekTimeoutInMilliseconds, SelectMode.SelectRead);
                    if (hasPayload)
                    {
                        bytesRead = m_Client.ReceiveFrom(connectionState.Buffer, 0, connectionState.Buffer.Length, SocketFlags.None, ref remoteendpoint);
                    }
                }
            }

            if ((bytesRead > 0) && (remoteendpoint != null))
            {
                this.OnMessageReceived(bytesRead, connectionState, remoteendpoint);
            }

            if (connectionState != null)
            {
                connectionState.Dispose();
            }
        }

        /// <summary>
        /// When a message is received on the specified UDP Multicast address/port.
        /// </summary>
        /// <remarks>
        /// Assumed to be not running under a thread safe context. As long as we don's access
        /// the socket referenced in the <see cref="ConnectionState"/> class we should be ok.
        /// <para></para>
        /// The buffer element is fine to access as it is created per call, regardless of which
        /// thread context has entered the ListenForMessage() method.
        /// </remarks>
        /// <param name="receivedByteCount">
        /// The number of bytes available to read.
        /// </param>
        /// <param name="state">
        /// The buffer state. 
        /// </param>
        /// <param name="remoteEndpoint">
        /// The endpoint that the data was received from.
        /// </param>
        private void OnMessageReceived(int receivedByteCount, ConnectionState state, EndPoint remoteEndpoint)
        {
            if (receivedByteCount > 0)
            {
                // Copy message buffer as the smallest size it can be to accomodate message and send it the message handler.
                var message = new byte[receivedByteCount];
                try
                {
                    Array.Copy(state.Buffer, message, receivedByteCount);
                    m_MessageHandler.HandleMessage(message);
                }
                finally
                {
                }
            }
        }
    }
}
