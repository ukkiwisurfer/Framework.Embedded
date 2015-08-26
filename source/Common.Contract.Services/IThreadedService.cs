namespace Ignite.Infrastructure.Micro.Contract.Services
{
    public interface IThreadedService : IService
    {
        /// <summary>
        /// The identifier of the service.
        /// </summary>
         string ServiceId{ get; }
    }
}