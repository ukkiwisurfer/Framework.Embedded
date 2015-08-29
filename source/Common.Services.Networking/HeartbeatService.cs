
namespace Ignite.Framework.Micro.Common.Services.Networking
{
    using Ignite.Framework.Micro.Common.Networking;

    /// <summary>
    /// Service that listens for heartbeat messages from a server.
    /// </summary>
    /// <remarks>
    /// If after a certain period of time no heartbeat message has been
    /// detected, the service attmpts to force a reboot of the device.
    /// </remarks>
    public class HeartbeatService : ThreadedService
    {
        private TcpSocket m_Socket;

        private bool m_IsActive;
        /// <summary>
        /// See <see cref="ThreadedService.IsServiceActive"/> for more details.
        /// </summary>
        public override bool IsServiceActive
        {
            get { return m_IsActive; }
        }

        /// <summary>
        /// Initialises an instance of the <see cref="HeartbeatService"/> class.
        /// </summary>
        /// <param name="socket"></param>
        public HeartbeatService(TcpSocket socket)
        {
            m_Socket = socket;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void DoWork()
        {
            
        }

        protected override void OnOpening()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnClosing()
        {
            throw new System.NotImplementedException();
        }

      
    }
}
