
namespace Ignite.Framework.Micro.Common.Services
{
    using System.Reflection;

    using Ignite.Framework.Micro.Common.Core.Resources;
    
    using Microsoft.SPOT;

    internal class ServicesResourceLoader : ResourceLoader, IResourceLoader
    {
        /// <summary>
        /// Initialises an instance of the <see cref="ResourceLoader"/> class.
        /// </summary>
        public ServicesResourceLoader() : base("Ignite.Framework.Micro.Common.Services.Resources", Assembly.GetExecutingAssembly())
        {
        }

        /// <summary>
        /// Returns the string from the resource file.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public string GetString(Resources.StringResources identifier)
        {
            return ResourceUtility.GetObject(ResourceManager, identifier) as string;
        }
    }
}
