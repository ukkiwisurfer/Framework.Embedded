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
    /// Provides the means to start and stop multiple services.
    /// </summary>
    public interface IServiceHost : IDisposable
    {
        /// <summary>
        /// Starts all services.
        /// </summary>
        void Start();

        /// <summary>
        /// Starts a specific service.
        /// </summary>
        /// <param name="serviceName">
        /// THe name of the service to start.
        /// </param>
        void Start(string serviceName);

        /// <summary>
        /// Stops all services.
        /// </summary>
        void Stop();

        /// <summary>
        /// Stops a specific service.
        /// </summary>
        /// <param name="serviceName">
        /// THe name of the service to stop.
        /// </param>
        void Stop(string serviceName);

        /// <summary>
        /// Adds a new instance of a service. 
        /// </summary>
        /// <param name="service">
        /// The service to add.
        /// </param>
        void AddService(IThreadedService service);

        /// <summary>
        /// Removes all previously added services
        /// </summary>
        void Clear();

        /// <summary>
        /// Returns the named service managed by the host.
        /// </summary>
        /// <param name="serviceName">
        /// The name of the service to query for.
        /// </param>
        /// <returns>
        /// The service instance if found, otherwise null.
        /// </returns>
        IThreadedService GetServiceByName(string serviceName);
    }
}
