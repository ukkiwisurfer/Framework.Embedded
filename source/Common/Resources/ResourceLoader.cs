
namespace Ignite.Framework.Micro.Common.Core.Resources
{
    using System;
    using System.Reflection;
    using System.Resources;

    using Microsoft.SPOT;

    public class ResourceLoader
    {
        private readonly ResourceManager m_Manager;
        private readonly Type m_ResourceType;

        /// <summary>
        /// INitialises an instance of the <see cref="ResourceLoader"/> class.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="assembly"></param>
        public ResourceLoader(string baseName, Assembly assembly, Type resourceType)
        {
            m_Manager = new ResourceManager(baseName,assembly);
            m_ResourceType = resourceType;
        }

        /// <summary>
        /// Returns the string from the resource file.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public string GetString(ResourceIdentifier identifier)
        {
            return ResourceUtility.GetObject(m_Manager, identifier) as string;
        }

    }

    /// <summary>
    /// Stub for resource identifiers.
    /// </summary>
    public enum ResourceIdentifier : short
    {
    
    }
}
