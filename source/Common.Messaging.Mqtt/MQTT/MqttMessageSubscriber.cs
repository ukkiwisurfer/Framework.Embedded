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

    using uPLibrary.Networking.M2Mqtt;
    using uPLibrary.Networking.M2Mqtt.Messages;

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Messaging;

    /// <summary>
    /// Processes incoming messages from an MQTT broker.
    /// </summary>
    public class MqttMessageSubscriber : IMessageSubscriber
    {
        private byte QOS = 1;

        private readonly MqttConnection m_Connection;
        private readonly IMessageHandler m_MessageHandler;
        private readonly string m_TopicName;
        private readonly string m_Name;
        private MqttClient m_Session;
        private string m_ConnectionId;
        private bool m_IsDisposed;
        private bool m_IsConnected;

        /// <summary>
        /// Returns the unique identifier of the connection.
        /// </summary>
        public string ConnectionId
        {
            get { return m_ConnectionId; }
        }

        /// <summary>
        /// Indicates whether the connection to the AMQP server is established.
        /// </summary>
        public bool IsConnected
        {
            get { return m_IsConnected; }
            private set { m_IsConnected = value; }
        }

        /// <summary>
        /// The maximum number of messages to receive concurrently.
        /// </summary>
        public int WindowSize { get; set; }

        /// <summary>
        /// Initialises an instance of the <see cref="MqttMessagePublisher"/> class.
        /// </summary>
        /// <param name="connection">
        /// The AMQP connection to use.
        /// </param>
        /// <param name="topicName">
        /// The topic name to publish to.
        /// </param>
        /// <param name="name">
        /// The unique name to associate with the link used to send messages on.
        /// </param>
        /// <param name="handler">
        ///Processes incoming messages from the MQTT broker.
        /// </param>
        public MqttMessageSubscriber(MqttConnection connection, string topicName, string name, IMessageHandler handler)
        {
            connection.ShouldNotBeNull();
            topicName.ShouldNotBeEmpty();
            name.ShouldNotBeEmpty();

            m_ConnectionId = string.Empty;
            m_Connection = connection;
            m_TopicName = topicName;
            m_Name = name;
            m_MessageHandler = handler;
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
        /// Enrols the subscriber in receiving messages for a given topic.
        /// </summary>
        public void Subscribe()
        {
            Subscribe(m_TopicName);
        }

        /// <summary>
        /// Enrols the subscriber in receiving messages for a given topic.
        /// </summary>
        /// <param name="topicName">
        /// The name of the MQ topic to subscribe to.
        /// </param>
        public void Subscribe(string topicName)
        {
            topicName.ShouldNotBeEmpty();

            try
            {
                if (IsConnected)
                {
                    m_Session.Subscribe(new[] { topicName }, new[] { QOS });
                }
            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// Stops the processing of messages sent from a MQTT broker.
        /// </summary>
        public void Unsubscribe()
        {
            Unsubscribe(m_TopicName);
        }

        /// <summary>
        /// Stops the processing of messages sent from a MQTT broker.
        /// </summary>
        public void Unsubscribe(string topicName)
        {
            topicName.ShouldNotBeEmpty();

            try
            {
                if (IsConnected)
                {
                    m_Session.Unsubscribe(new[] { topicName });
                }
            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// Attempts to connect to a AMQP server.
        /// </summary>
        public void Connect()
        {
            try
            {
                if (!IsConnected)
                {
                    if (!m_Connection.IsConnected)
                    {
                        m_Connection.Connect();

                        m_ConnectionId = m_Connection.ConnectionId;
                        
                        m_Session = m_Connection.Session;
                        m_Session.MqttMsgPublishReceived += OnMessage;
                    }

                    IsConnected = true;
                }
            }
            catch (Exception e)
            {
                Disconnect();
            }
        }

        /// <summary>
        /// ON receipt of a message from a MQTT broker, process it.
        /// </summary>
        /// <param name="sender">
        /// The MQTT session that raised the event.
        /// </param>
        /// <param name="mqttMsgPublishEventArgs">
        /// The message details that were received.
        /// </param>
        private void OnMessage(object sender, MqttMsgPublishEventArgs mqttMsgPublishEventArgs)
        {
            throw new NotImplementedException();
            //m_MessageHandler.HandleMessage(ref mqttMsgPublishEventArgs.Message);
        }

        /// <summary>
        /// Disconnects from a AMQP server.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (IsConnected)
                {
                    m_Session.MqttMsgPublishReceived -= OnMessage;
                    m_Session = null;
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                IsConnected = false;
            }
        }

    }
}
