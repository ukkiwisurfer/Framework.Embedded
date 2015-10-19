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

namespace Ignite.Framework.Micro.Common.Contract.Messaging
{
    /// <summary>
    /// Provides support for connection to a MQ (message queueing) broker.
    /// </summary>
    public interface IQueuedConnection
    {
        /// <summary>
        /// The idenifier of the the client.
        /// </summary>
        string ConnectionId { get; }

        /// <summary>
        /// Indicates whether the connection to the AMQP server is open.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Opens the connection to the queue,
        /// </summary>
        void Connect();

        /// <summary>
        /// Closes the connection to the queue.
        /// </summary>
        void Disconnect();
    }
}