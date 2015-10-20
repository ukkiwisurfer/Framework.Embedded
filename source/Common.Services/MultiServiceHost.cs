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

using Ignite.Framework.Micro.Common.Core.Extensions;

namespace Ignite.Framework.Micro.Common.Services
{
    using System;
    using System.Collections;

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Logging;
    using Ignite.Framework.Micro.Common.Contract.Services;

    /// <summary>
    /// Service host that supports multiple threaded services. 
    /// </summary>
    public class MultiServiceHost : IServiceHost
    {
        private IResourceLoader m_ResourceLoader;
        private readonly Hashtable m_ServiceEntries;
        private readonly ILogger m_Logger;
        private bool m_IsDisposed;

        /// <summary>
        /// Initialises an instance of a <see cref="MultiServiceHost"/> class.
        /// </summary>
        /// <param name="logger">
        /// Logging provider.
        /// </param>
        public MultiServiceHost(ILogger logger)
        {
            logger.ShouldNotBeNull();

            m_ServiceEntries = new Hashtable();
            m_Logger = logger;
            m_ResourceLoader = new ServicesResourceLoader();
        }

        /// <summary>
        /// See <see cref="IDisposable.Dispose"/> for more details
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Starts all hosts.
        /// </summary>
        public void Start()
        {
            m_Logger.Info(m_ResourceLoader.GetString(Resources.StringResources.StartingHosts), m_ServiceEntries.Count);
            foreach (var entry in m_ServiceEntries.Values)
            {
                var cast = entry as ServiceEntry;
                if (cast != null)
                {
                    cast.Service.Start();
                }
            }
        }

        /// <summary>
        /// Stops all hosts.
        /// </summary>
        public void Stop()
        {
            m_Logger.Info(m_ResourceLoader.GetString(Resources.StringResources.StoppingHosts), m_ServiceEntries.Count);
            foreach (var entry in m_ServiceEntries.Values)
            {
                var cast = entry as ServiceEntry;
                if (cast != null)
                {
                    cast.Service.Stop();
                }
            }
        }

        /// <summary>
        /// Adds a new instance of a service. 
        /// </summary>
        /// <param name="service">
        /// The service instance to add.
        /// </param>
        public void AddService(IThreadedService service)
        {
            service.ShouldNotBeNull();

            var entry = new ServiceEntry(service.ServiceId, service); 
            m_ServiceEntries.Add(entry.ServiceName, entry);

            m_Logger.Info(m_ResourceLoader.GetString(Resources.StringResources.AddedNewHost), service.ServiceId );
        }

        /// <summary>
        /// Clears all services from the host.
        /// </summary>
        public void Clear()
        {
            m_Logger.Info(m_ResourceLoader.GetString(Resources.StringResources.ClearingHosts), m_ServiceEntries.Count);

            foreach (var entry in m_ServiceEntries.Values)
            {
                var cast = entry as ServiceEntry;
                if (cast != null)
                {
                    try
                    {
                        cast.Dispose();
                    }
                    catch (Exception ex)
                    {
                        m_Logger.Error(m_ResourceLoader.GetString(Resources.StringResources.ExceptionOccurred), ex);
                    }
                }
            }

            m_ServiceEntries.Clear();
            m_Logger.Info(m_ResourceLoader.GetString(Resources.StringResources.AllHostsRemoved));
        }

        /// <summary>
        /// Disposes of any unmanaged resources.
        /// </summary>
        /// <remarks>
        /// Flushes any pending readings prior to disposal. 
        /// </remarks>
        /// <param name="isDisposing">
        /// Indicates whether the disposal is deterministic or not.
        /// </param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (!m_IsDisposed)
            {
                try
                {
                    if (isDisposing)
                    {
                        this.Clear();
                    }
                }
                finally
                {
                    m_IsDisposed = true;
                }
            }
        }

        /// <summary>
        /// Returns the first service with the matching name as the supplied criteria.
        /// </summary>
        /// <param name="serviceName">
        /// The name of the service to search for.
        /// </param>
        /// <returns>
        /// The service matching the given search criteria. 
        /// </returns>
        public IThreadedService GetServiceByName(string serviceName)
        {
            IThreadedService service = null;

            try
            {
                var matches = m_ServiceEntries.Where(x => x.Equals(serviceName));
                service = matches.First() as IThreadedService;
            }
            catch (Exception)
            {                
            }

            return service;
        }
    }
}
