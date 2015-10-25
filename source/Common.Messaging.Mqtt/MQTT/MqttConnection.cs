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

namespace Ignite.Framework.Micro.Common.Messaging.MQTT
{
    using System;

    using Microsoft.SPOT;

    using uPLibrary.Networking.M2Mqtt;

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Messaging;

    /// <summary>
    /// Provides a connection to an MQTT broker.
    /// </summary>
    public class MqttConnection : IQueuedConnection, IDisposable
    {
        private readonly RegistrationData m_RegistrationData;
        private readonly EventHandler m_OnClosedConnection;
        private readonly string m_ServiceName;
        private readonly object m_Synclock;
        private MqttClient m_Connection;
        private bool m_IsDisposed;
        private bool m_IsConnected;
        private bool m_IsConnecting;
        private string m_ConnectionId;

        /// <summary>
        /// See <see cref="IQueuedConnection.IsConnected"/> for more details.
        /// </summary>
        public bool IsConnected
        {
            get { return m_IsConnected; }
        }

        /// <summary>
        /// The identifier for the MQTT client.
        /// </summary>
        public string ConnectionId
        {
            get { return m_ConnectionId; }
        }

        /// <summary>
        /// The MQTT session.
        /// </summary>
        internal MqttClient Session
        {
            get {  return m_Connection; }
        }

    /// <summary>
        /// Initialises an instance of the <see cref="MqttConnection"/> class.
        /// </summary>
        /// <param name="registrationData"></param>
        public MqttConnection(RegistrationData registrationData)
        {
            registrationData.ShouldNotBeNull();

            m_RegistrationData = registrationData;
            m_Synclock = new object();
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
        /// Opens a connection to the MQTT broker.
        /// </summary>
        public void Connect()
        {
            lock (m_Synclock)
            {
                try
                {
                    if (!m_IsConnected)
                    {
                        m_ConnectionId = Guid.NewGuid().ToString();

                        m_Connection = new MqttClient(m_RegistrationData.IPAddress);
                        m_Connection.ConnectionClosed += OnClosedConnection;
                        m_Connection.Connect(m_ConnectionId, m_RegistrationData.Username, m_RegistrationData.Password);

                        m_IsConnected = true;
                    }

                }
                catch (Exception e)
                {
                    m_IsConnected = false;
                }
                finally
                {
                    m_IsConnecting = false;
                }

            }
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
        private void OnClosedConnection(object sender, EventArgs error)
        {
            Disconnect();

            if (m_OnClosedConnection != null)
            {
                m_OnClosedConnection(this, error);
            }
        }

        /// <summary>
        /// Closes a connection to the MQTT broker.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (m_Connection != null) m_Connection.Disconnect();
            }
            finally
            {
                m_Connection = null;
                m_IsConnected = false;
            }
        }


    }
}
