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

using System.IO;

namespace Ignite.Framework.Micro.Common.Contract.Messaging
{
    using System;

    /// <summary>
    /// Provides the means to publish messages to a MQ broker.
    /// </summary>
    public interface IMessagePublisher : IQueuedConnection, IDisposable
    {
        /// <summary>
        /// Publishes a message to a MQ broker.
        /// </summary>
        /// <param name="payload">
        /// The payload to publish.
        /// </param>
        void Publish(ref byte[] payload);

        /// <summary>
        /// Publishes a message to a MQ broker.
        /// </summary>
        /// <param name="payload">
        /// The payload to publish.
        /// </param>
        void Publish(MemoryStream payload);

        /// <summary>
        /// Publishes a message to a MQ broker.
        /// </summary>
        /// <param name="payload">
        /// The payload to publish.
        /// </param>
        /// <param name="isDurable">
        /// Indicates whether the message should be persisted by the queue.
        /// </param>
        void Publish(ref byte[] payload, bool isDurable);

        /// <summary>
        /// Publishes a message to a MQ broker.
        /// </summary>
        /// <param name="payload">
        /// The payload to publish.
        /// </param>
        /// <param name="isDurable">
        /// Indicates whether the message should be persisted by the queue.
        /// </param>
        void Publish(MemoryStream payload, bool isDurable);

    }
}