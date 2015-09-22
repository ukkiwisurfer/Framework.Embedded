
namespace Ignite.Framework.Micro.Common.Services
{
    using System.Reflection;
    using System.Resources;

    using Microsoft.SPOT;

    internal class ResourceLoader : IResourceLoader
    {
        private readonly ResourceManager m_Manager;

        /// <summary>
        /// INitialises an instance of the <see cref="ResourceLoader"/> class.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="assembly"></param>
        public ResourceLoader(string baseName, Assembly assembly)
        {
            m_Manager = new ResourceManager(baseName,assembly);
        }

        /// <summary>
        /// Returns the string from the resource file.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public string GetString(Resources.StringResources identifier)
        {
            return ResourceUtility.GetObject(m_Manager, identifier) as string;
        }

    }
}
