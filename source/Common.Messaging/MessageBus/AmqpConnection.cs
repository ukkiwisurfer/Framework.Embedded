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

namespace Ignite.Framework.Micro.Common.Messaging.MessageBus
{
    using System;
    using Amqp;
    using Amqp.Framing;
    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Messaging;
    using Microsoft.SPOT;

    /// <summary>
    /// Processes incoming messages and dispatches them via an
    /// AMQP endpoint.
    /// </summary>
    public class AmqpConnection : IQueuedConnection, IDisposable
    {
        private readonly QueueEndpointAddress m_Address;
        private readonly EventHandler m_OnClosedConnection;
        private readonly string m_ServiceName;
        private Connection m_Connection;
        private Session m_Session;
        private bool m_IsDisposed;

        /// <summary>
        /// The AMQP session that is required to publish messages on.
        /// </summary>
        internal Session Session
        {
            get {  return m_Session; }
        }

        private bool m_IsConnected;
        /// <summary>
        /// See <see cref="IQueuedConnection.IsConnected"/> for more details.
        /// </summary>
        public bool IsConnected
        {
            get { return m_IsConnected; }
        }

        private string m_ConnectionId;
        /// <summary>
        /// The identifier for the MQTT client.
        /// </summary>
        public string ConnectionId
        {
            get { return m_ConnectionId; }
        }


        /// <summary>
        /// Initialises an instance of the <see cref="AmqpConnection"/> class.
        /// </summary>
        /// <param name="registration">
        /// Details required to connect to the queued message server.
        /// </param>
        public AmqpConnection(RegistrationData registration)
        {
            registration.ShouldNotBeNull();

            m_Address = registration.Address;
        }

        /// <summary>
        /// Initialises an instance of the <see cref="AmqpConnection"/> class.
        /// </summary>
        /// <param name="registration">
        /// Details required to connect to the queued message server.
        /// </param>
        /// <param name="onCloseEventHandler">
        /// Event handler to fire when the connection is closed.
        /// </param>
        public AmqpConnection(RegistrationData registration, EventHandler onCloseEventHandler) : this(registration)
        {
            m_OnClosedConnection = onCloseEventHandler;
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
                   Disconnect();
                }

                m_IsDisposed = true;
            }
        }

        /// <summary>
        /// Opens a connection to the AMQP server.
        /// </summary>
        public void Connect()
        {
            m_ConnectionId = Guid.NewGuid().ToString();

            var address = new Address(m_Address.GetUrl());

            m_Connection = new Connection(address);
            m_Connection.Closed += OnClosedConnection;

            m_Session = new Session(m_Connection);

            m_IsConnected = true;
        }

        /// <summary>
        /// Event handler that fires when an AMQP connection is closed.
        /// </summary>
        /// <param name="sender">
        /// The AMQP connection that is propogating the closed event.
        /// </param>
        /// <param name="error">
        /// The AMQP <see cref="Error"/> event that caused the connection to close (if one exists). 
        /// </param>
        private void OnClosedConnection(AmqpObject sender, Error error)
        {
            Disconnect();
            if (m_OnClosedConnection != null)
            {
                m_OnClosedConnection(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Closes the connection to the AMQP server.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                m_Session.Close();
                m_Connection.Close();
            }
            finally
            {
                m_Session = null;
                m_Connection = null;

                m_IsConnected = false;
            }
        }
    }
}
