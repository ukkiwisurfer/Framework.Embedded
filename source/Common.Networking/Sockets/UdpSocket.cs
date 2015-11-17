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

using System.Threading;
using Ignite.Framework.Micro.Common.Contract.Logging;
using Ignite.Framework.Micro.Common.Core.Threading;
using Ignite.Framework.Micro.Common.Logging;

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
        private Socket m_Client;
        private readonly IPEndPoint m_Endpoint;
        private readonly IMessageHandler m_MessageHandler;
        private readonly object m_SyncLock;
        private bool m_IsOpen;
        private bool m_IsDisposed;
        private ILogger m_Logger;

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
        public UdpSocket(ILogger logger, string multicastAddress, int port, IMessageHandler handler)
        {
            m_Endpoint = new IPEndPoint(IPAddress.Parse(multicastAddress), port);
            m_MessageHandler = handler;

            m_SyncLock = new object();
            m_Logger = logger;

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
            if (!m_IsOpen)
            {
                lock (m_SyncLock)
                {
                    m_Logger.Debug("Opening UDP socket.");

                    try
                    {
                        m_Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        m_Client.Bind(new IPEndPoint(IPAddress.Any, m_Endpoint.Port));
                        m_IsOpen = true;
                    }
                    catch (Exception ex)
                    {
                        m_Logger.Error("Exception occurred opening UDP socket.", ex);
                        m_IsOpen = false;
                    }
                }
            }
        }

        /// <summary>
        /// Ends the client connection.
        /// </summary>
        public void Close()
        {
            lock (m_SyncLock)
            {
                m_Logger.Debug("Closing UDP socket.");

                try
                {
                    m_Client.Close();
                    m_IsOpen = false;
                }
                catch (Exception ex)
                {
                    m_Logger.Error("Exception occurred closing UDP socket.", ex);
                    m_IsOpen = false;
                }
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
            bool hasPayload = false;

            lock (m_SyncLock)
            {
                try
                {
                    if (!m_IsOpen) Open();

                    if (m_IsOpen)
                    {
                        hasPayload = m_Client.Poll(PeekTimeoutInMilliseconds, SelectMode.SelectRead);
                    }
                    else
                    {
                        m_Logger.Debug("UDP socket failed to open. Polling for data unavailable.");
                    }
                }
                catch (Exception ex)
                {
                    m_Logger.Error("Exception occurred polling UDP socket.", ex);
                    Close();
                }
            }

            return hasPayload;
        }

        /// <summary>
        /// Checks to see if there is any data on the socket to process.
        /// </summary>
        /// <remarks>
        /// Will not block indefinitely while waiting for data or throwing 
        /// an exception if a timeout was specified on the read.
        /// </remarks>
        public void ProcessMessage()
        {
            MessageRequest messageRequest = null;
            int bytesRead = 0;

            var logger = new ConsoleLogger();

            lock (m_SyncLock)
            {
                //logger.LogFreeMemory();

                if (!m_IsOpen) Open();
                if (m_IsOpen)
                {
                    logger.LogFreeMemory();

                    messageRequest = new MessageRequest(m_MessageHandler, BufferSizeInBytes);

                    try
                    {
                        bool hasPayload = m_Client.Poll(PeekTimeoutInMilliseconds, SelectMode.SelectRead);
                        if (hasPayload)
                        {
                            bytesRead = m_Client.Receive(messageRequest.Buffer, 0, messageRequest.Buffer.Length, SocketFlags.None);
                        }
                    }
                    catch (Exception ex)
                    {
                        m_Logger.Error("Exception occurred receiving data from UDP socket.", ex);
                        Close();
                    }
                }
            }

            if (bytesRead > 0)
            {
                OnMessageReceived(bytesRead, messageRequest);
            }

            if (messageRequest != null)
            {
                messageRequest.Dispose();
            }

            //logger.LogFreeMemory();
        }

        /// <summary>
        /// When a message is received on the specified UDP Multicast address/port.
        /// </summary>
        /// <remarks>
        /// Assumed to be not running under a thread safe context. As long as we don's access
        /// the socket referenced in the <see cref="MessageRequest"/> class we should be ok.
        /// <para></para>
        /// The buffer element is fine to access as it is created per call, regardless of which
        /// thread context has entered the ProcessMessage() method.
        /// </remarks>
        /// <param name="receivedByteCount">
        /// The number of bytes available to read.
        /// </param>
        /// <param name="state">
        /// The buffer state. 
        /// </param>
        private void OnMessageReceived(int receivedByteCount, MessageRequest state)
        {
            if (receivedByteCount > 0)
            {
                try
                {
                    state.Execute();
                }
                catch (Exception ex)
                {
                    m_Logger.Error("Exception detected handling message.", ex);
                }
            }
        }

        /// <summary>
        /// Sends a message via UDP on the specified UDP Multicast address/port.
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(byte[] message)
        {
            m_Client.Send(message);
        }
    }
}
