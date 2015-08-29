
namespace Ignite.Framework.Micro.Common.Services.Networking
{
    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Networking;
    using Ignite.Framework.Micro.Common.Services;

    /// <summary>
    /// Listens for UDP broadcasts via a <see cref="UdpSocket"/> instance and captures any incoming messages. 
    /// </summary>
    public class UdpListenerService : ThreadedService
    {
        private readonly UdpSocket m_Socket;

        /// <summary>
        /// Initialises an instance of the <see cref="UdpListenerService"/> class.
        /// </summary>
        /// <param name="socket">
        /// The socket instance to use for listening to Owl electricity monitoring broadcasts.
        /// </param>
        public UdpListenerService(UdpSocket socket)
        {
            socket.ShouldNotBeNull();
            m_Socket = socket;
        }

        /// <summary>
        /// See <see cref="ThreadedService.CheckIfWorkExists"/> for more details.
        /// </summary>
        /// <param name="hasWork">
        /// Indicates if work has been detected by the caller.
        /// </param>
        public override void CheckIfWorkExists(bool hasWork = false)
        {
            var workExists = m_Socket.CheckForIncomingData();
            if (workExists)
            {
                this.SignalWorkToBeDone();
            }
        }

        /// <summary>
        /// See <see cref="ThreadedService.DoWork"/> for more details.
        /// </summary>
        /// <remarks>
        /// Checks to see if there is any data the socket has received to process.
        /// Blocks for the period defined by the <see cref="UdpSocket.PeekTimeoutInMilliseconds"/> 
        /// property.
        /// <para></para>
        /// Be aware there is a cumulative period for waiting. This method is only called
        /// when 
        /// </remarks>
        protected override void DoWork()
        {
            m_Socket.ListenForMessage();
            this.SignalWorkCompleted();
        }

        /// <summary>
        /// See <see cref="ThreadedService.OnOpening"/> for more details.
        /// </summary>
        protected override void OnOpening()
        {
            m_Socket.Open();
        }

        /// <summary>
        /// See <see cref="ThreadedService.OnClosing"/> for more details.
        /// </summary>
        protected override void OnClosing()
        {
            m_Socket.Close();
        }

        /// <summary>
        /// See <see cref="ThreadedService.IsServiceActive"/> for more details.
        /// </summary>
        public override bool IsServiceActive
        {
            get { return m_Socket.IsOpen; }
        }
    }
}
