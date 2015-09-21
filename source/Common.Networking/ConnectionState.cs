
namespace Ignite.Framework.Micro.Common.Networking
{
    using System;
    using System.Net.Sockets;

    /// <summary>
    /// Holds connection state.
    /// </summary>
    public class ConnectionState : IDisposable
    {
        private byte[] m_Buffer;
        private readonly Socket m_Socket;
        private bool m_IsDisposed;

        /// <summary>
        /// Buffer for receiving incoming data.
        /// </summary>
        public byte[] Buffer
        {
            get { return m_Buffer; }
        }

        /// <summary>
        /// The socket on which the data was received.
        /// </summary>
        public Socket Socket
        {
            get { return m_Socket; }   
        }

        /// <summary>
        /// Creates an instance of the <see cref="ConnectionState"/> class.
        /// </summary>
        /// <param name="socket">
        /// The network socket to read from.
        /// </param>
        /// <param name="bufferSize">
        /// The maximum size of the message buffer.
        /// </param>
        public ConnectionState(Socket socket, int bufferSize = 1024)
        {
            m_Socket = socket;
            m_Buffer = new byte[bufferSize];
        }

        /// <summary>
        /// Disposes of any managed or unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of any managed or unmanaged resources.
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
                    m_Buffer = null;
                }

                m_IsDisposed = true;
            }
        }
    }
}
