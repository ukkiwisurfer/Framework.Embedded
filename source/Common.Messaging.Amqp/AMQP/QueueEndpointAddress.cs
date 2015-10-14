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

namespace Ignite.Framework.Micro.Common.Messaging.AMQP
{

    using Ignite.Framework.Micro.Common.Core;

    /// <summary>
    /// Connection details for connecting to an AMQP endpoint.
    /// </summary>
    public class QueueEndpointAddress
    {
        /// <summary>
        /// The name of the host for the MQ server.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The name of the host for the MQ server.
        /// </summary>
        public string VirtualHost { get; set; }

        /// <summary>
        /// The IP address of the MQ server.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// The port address for the MQ server.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The username to connect to the MQ server.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The password to connect to the MQ server.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The MQ queue name.
        /// </summary>
        public string TargetName { get; set; }

        /// <summary>
        /// Returns a AMQP specific connection URL.
        /// </summary>
        /// <remarks>
        /// Follows the pattern: amqp://username:password@host:port
        /// </remarks>
        /// <returns>
        /// A AMQP connection string as a URL.
        /// </returns>
        public string GetUrl()
        {
            var url = StringUtility.Format("amqp://{0}:{1}@{2}:{3}", Username, Password, IPAddress, Port);
            return url;
        }
    }
}
