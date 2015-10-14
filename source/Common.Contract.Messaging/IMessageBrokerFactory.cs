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
    using System;

    /// <summary>
    /// Factory for creating client connections to a message bus server.
    /// </summary>
    public interface IMessageBrokerFactory : IDisposable
    {
        /// <summary>
        /// Creates a message bus client.
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="linkName"></param>
        /// <returns>
        /// An instance of a message bus publisher.
        /// </returns>
        IMessagePublisher BuildPublisher(string topicName, string linkName);

        /// <summary>
        /// Creates a message bus client.
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="linkName"></param>
        /// <param name="messageHandler"></param>
        /// <returns>
        /// An instance of a message bus subscriber.
        /// </returns>
        IMessageSubscriber BuildSubscriber(string topicName, string linkName, IMessageHandler messageHandler);
    }
}