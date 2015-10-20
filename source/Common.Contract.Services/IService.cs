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

namespace Ignite.Framework.Micro.Common.Contract.Services
{
    using System;

    /// <summary>
    /// Defines the operations applicable to managing a service instance.
    /// </summary>
    public interface IService : IDisposable
    {
        /// <summary>
        /// The identifier of the service.
        /// </summary>
        string ServiceId { get; }

        /// <summary>
        /// The service name to associate with the service.
        /// </summary>
        string ServiceName { get; set; }

        /// <summary>
        /// Indicates whether the servce is running
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Starts the host.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the host.
        /// </summary>
        void Stop();
    }
}