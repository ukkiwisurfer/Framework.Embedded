namespace Ignite.Framework.Micro.Common.Contract.Services
{
    public interface IThreadedService : IService
    {
        /// <summary>
        /// The identifier of the service.
        /// </summary>
         string ServiceId{ get; }
    }
}