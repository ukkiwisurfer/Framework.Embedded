
using Ignite.Framework.Micro.Common.Contract.Messaging;

namespace Ignite.Framework.Micro.Common.Networking
{
    using System;
    using System.Net.Sockets;

    /// <summary>
    /// Holds connection state.
    /// </summary>
    public class MessageRequest : IDisposable
    {
        private byte[] m_Buffer;
        private readonly IMessageHandler m_Handler;
        private bool m_IsDisposed;

        /// <summary>
        /// Buffer for receiving incoming data.
        /// </summary>
        public byte[] Buffer
        {
            get { return m_Buffer; }
            set { m_Buffer = value; }
        }

        /// <summary>
        /// Creates an instance of the <see cref="MessageRequest"/> class.
        /// </summary>
        /// <param name="socket">
        /// The network socket to read from.
        /// </param>
        /// <param name="bufferSize">
        /// The maximum size of the message buffer.
        /// </param>
        public MessageRequest(IMessageHandler handler, int bufferSize = 1024)
        {
            m_Handler = handler;
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
        /// 
        /// </summary>
        public void Execute()
        {
            m_Handler.HandleMessage(ref m_Buffer);
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
