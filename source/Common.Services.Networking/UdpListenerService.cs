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

namespace Ignite.Framework.Micro.Common.Services.Networking
{
    using System;

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Networking;
    using Ignite.Framework.Micro.Common.Services;
    using Ignite.Framework.Micro.Common.Contract.Logging;

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
        public UdpListenerService(UdpSocket socket) : base(typeof(UdpListenerService))
        {
            socket.ShouldNotBeNull();
            m_Socket = socket;
        }

        /// <summary>
        /// Initialises an instance of the <see cref="UdpListenerService"/> class.
        /// </summary>
        /// <param name="socket">
        /// The socket instance to use for listening to Owl electricity monitoring broadcasts.
        /// </param>
        /// <param name="logger"></param>
        public UdpListenerService(ILogger logger, UdpSocket socket) : base(logger, typeof(UdpListenerService))
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
            LogDebug("Checking for incoming data on UDP socket.");

            var workExists = m_Socket.CheckForIncomingData();
            if (workExists)
            {
                LogDebug("Incoming data detected on UDP socket.");
                SignalWorkToBeDone();
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
            LogDebug("Commencing processing.");

            try
            {
                m_Socket.ProcessMessage();
            }
            catch (Exception ex)
            {
                LogError("Exception occurred while processing UDP data.", ex);
            }
            finally
            {
                SignalWorkCompleted();
                LogDebug("Processing complete.");
            }
        }

        /// <summary>
        /// See <see cref="ThreadedService.OnOpening"/> for more details.
        /// </summary>
        protected override void OnOpening()
        {
            base.OnOpening();
            m_Socket.Open();
        }

        /// <summary>
        /// See <see cref="ThreadedService.OnClosing"/> for more details.
        /// </summary>
        protected override void OnClosing()
        {
            m_Socket.Close();
            base.OnClosing();
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
