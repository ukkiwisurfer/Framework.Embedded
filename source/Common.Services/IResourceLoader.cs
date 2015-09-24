namespace Ignite.Framework.Micro.Common.Services
{
    using System;

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