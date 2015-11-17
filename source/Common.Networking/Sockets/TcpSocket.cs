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
    using System.Threading;

    using Ignite.Framework.Micro.Common.Contract.Messaging;

    /// <summary>
    /// Wraps a TCP socket.
    /// </summary>
    /// <remarks>
    /// Captures TCP packets and passes the received data to a <see cref="IMessageHandler"/> 
    /// instance for processing.
    /// </remarks>
    public class TcpSocket : IDisposable
    {
        private Thread m_ConnectThead;
        private readonly Socket m_Socket;
        private readonly ManualResetEvent m_ConnectedEvent;
        private readonly IMessageHandler m_MessageHandler;
        private readonly object m_SyncLock;
        private readonly string m_HostName;
        private readonly int m_Port;
        private bool m_IsOpen;
        private bool m_IsDisposed;


        /// <summary>
        /// Indicates whether the socket is open.
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
        /// The period to wait for a connection to establish in millseconds.
        /// </summary>
        /// <remarks>
        /// This cannot be set to a value less than 500 milliseconds. The .NET
        /// Micro framework will not honour a value less than this.
        /// </remarks>
        public int ConnectTimeoutInMilliseconds { get; set; }

        /// <summary>
        /// The period to wait for any incoming bytes in millseconds.
        /// </summary>
        /// <remarks>
        /// This cannot be set to a value less than 500 milliseconds. The .NET
        /// Micro framework will not honour a value less than this.
        /// </remarks>
        public int PeekTimeoutInMilliseconds { get; set; }


        /// <summary>
        /// Initialises an instance of the <see cref="TcpSocket"/> class.
        /// </summary>
        /// <param name="hostName">
        /// The name of the host to connect to.
        /// </param>
        /// <param name="port">
        /// The address of the port to connect to.
        /// </param>
        /// <param name="messageHandler">
        /// Handler for incoming data.
        /// </param>
        public TcpSocket(string hostName, int port, IMessageHandler messageHandler)
        {
            m_HostName = hostName;
            m_Port = port;
            m_MessageHandler = messageHandler;

            m_SyncLock = new object();
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_ConnectedEvent = new ManualResetEvent(false);
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
                    m_Socket.Close();

                    var disposable = m_Socket as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }

                m_IsDisposed = true;
            }
        }

        /// <summary>
        /// Attempts to open a socket connection.
        /// </summary>
        public void Open()
        {
            m_IsOpen = false;

            lock (m_SyncLock)
            {
                m_ConnectThead = new Thread(OnConnect);
                m_ConnectThead.Start();

                var status = m_ConnectedEvent.WaitOne(ConnectTimeoutInMilliseconds, false);
                if (!status)
                {
                    // Connection timeout.                
                }
                else m_IsOpen = true;

                try
                {
                    m_ConnectThead.Abort();
                }
                finally
                {
                    m_ConnectThead = null;
                }
            }
        }

        /// <summary>
        /// Closes the socket.
        /// </summary>
        public void Close()
        {
            lock(m_SyncLock)
            {
                m_Socket.Close();
                m_IsOpen = false;                
            }
        }

        /// <summary>
        /// Attempts to establish a connection to a TCP socket endpoint.
        /// </summary>
        private void OnConnect()
        {
            var endpoint = new IPEndPoint(IPAddress.Parse(m_HostName), m_Port);

            lock (m_SyncLock)
            {
                // Don't let the socket linger after closing and flush all messages.
                m_Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Linger, new byte[] { 0, 0, 0, 0 });
                m_Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                m_Socket.Connect(endpoint);

                m_ConnectedEvent.Set();
            }
        }

        /// <summary>
        /// Sends data via the TCP socket.
        /// </summary>
        /// <param name="buffer">
        /// The buffer to send.
        /// </param>
        public void Send(byte[] buffer)
        {
            lock (m_SyncLock)
            {
                m_Socket.Send(buffer, 0, buffer.Length, SocketFlags.None);
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
            lock (m_SyncLock)
            {
                bool hasPayload = m_Socket.Poll(PeekTimeoutInMilliseconds, SelectMode.SelectRead);
                if (hasPayload)
                {
                    var state = new MessageRequest(m_MessageHandler, BufferSizeInBytes);
                    EndPoint remoteendpoint = new IPEndPoint(IPAddress.Parse(m_HostName), m_Port);

                    int bytesRead = m_Socket.ReceiveFrom(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, ref remoteendpoint);

                    this.OnMessageReceived(bytesRead, state);
                }
            }
        }

        /// <summary>
        /// When a message is received on the specified UDP Multicast address/port.
        /// </summary>
        /// <remarks>
        /// Assumed to run under a thread-safe context.
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
                // Copy message buffer and send it the message handler.
                var message = new byte[receivedByteCount];
                Array.Copy(state.Buffer, message, receivedByteCount);

                m_MessageHandler.HandleMessage(ref message);
            }
        }
    }
}
