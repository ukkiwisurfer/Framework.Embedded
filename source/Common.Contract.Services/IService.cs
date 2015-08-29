namespace Ignite.Framework.Micro.Common.Contract.Services
{
    using System;

    public interface IService : IDisposable
    {/// <summary>
     /// Starts the host.
     /// </summary>
        void Start();

        /// <summary>
        /// Stops the host.
        /// </summary>
        void Stop();
    }
}