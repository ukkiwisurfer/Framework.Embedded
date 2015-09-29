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

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Messaging;

    /// <summary>
    /// Processes incoming messages and dispatches them via an
    /// AMQP endpoint.
    /// </summary>
    public class AmqpConnection : IQueuedConnection, IDisposable
    {
        private readonly QueueEndpointAddress m_Address;
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

        private string m_ClientId;
        /// <summary>
        /// The identifier for the MQTT client.
        /// </summary>
        public string ClientId
        {
            get { return m_ClientId; }
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
            m_ClientId = Guid.NewGuid().ToString();

            var address = new Address(m_Address.GetUrl());

            m_Connection = new Connection(address);
            m_Session = new Session(m_Connection);

            m_IsConnected = true;
        }

        /// <summary>
        /// Closes the connection to the AMQP server.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                m_Connection.Close();
                m_Session.Close();
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
