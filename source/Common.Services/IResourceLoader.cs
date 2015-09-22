namespace Ignite.Framework.Micro.Common.Services
{
    internal interface IResourceLoader
    {
        /// <summary>
        /// Loads an emebedded string resource.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        string GetString(Resources.StringResources identifier);
    }
}