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

    /// <summary>
    /// Describes registraton details to connect to a message queue server. 
    /// </summary>
    public class RegistrationData
    {
        /// <summary>
        /// The name of the host for the MQ server.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The IP address of the MQ server.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// The port for the MQ server.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The SSL port for the MQ server.
        /// </summary>
        public int SslPort { get; set; }

        /// <summary>
        /// The username to connect to the MQ server.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The password to connect to the MQ server.
        /// </summary>
        public string Password { get; set; }

    }
}
