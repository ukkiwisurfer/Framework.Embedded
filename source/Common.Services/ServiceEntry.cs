using System;

namespace Ignite.Framework.Micro.Common.Services
{
    using Ignite.Framework.Micro.Common.Contract.Services;

    /// <summary>
    /// Describes the name of a service and the service associated with it.
    /// </summary>
    internal class ServiceEntry
    {
        private readonly string m_ServiceName;
        private readonly IService m_Service;
        private bool m_IsDisposed;

        /// <summary>
        /// THe name of the service.
        /// </summary>
        public string ServiceName
        {
            get { return m_ServiceName; }
        }

        /// <summary>
        /// The service itself
        /// </summary>
        public IService Service
        {
            get { return m_Service; }
        }

        /// <summary>
        /// Initialises an instance of the <see cref="ServiceEntry"/> class.
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="service"></param>
        public ServiceEntry(string serviceName, IService service)
        {
            m_ServiceName = serviceName;
            m_Service = service;
        }

        /// <summary>
        /// See <see cref="IDisposable.Dispose"/> for more details.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Ensures both managed and unmanaged resources are disposed.
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
                    m_Service.Dispose();
                }
            }
        }
    }
}
