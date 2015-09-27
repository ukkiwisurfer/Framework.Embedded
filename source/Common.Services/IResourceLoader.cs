namespace Ignite.Framework.Micro.Common.Services
{

    /// <summary>
    /// Provides support to load embedded resources.
    /// </summary>
    internal interface IResourceLoader
    {
        /// <summary>
        /// Loads an embedded string resource.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        string GetString(Resources.StringResources identifier);

    }
}