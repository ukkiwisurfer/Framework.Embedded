
namespace Ignite.Framework.Micro.Common.Core.Resources
{
    using System.Reflection;
    using System.Resources;

    public class ResourceLoader
    {
        private readonly ResourceManager m_Manager;
        /// <summary>
        /// Provides access to the internal <see cref="ResourceManager"/> instance.
        /// </summary>
        protected ResourceManager ResourceManager
        {
            get {  return m_Manager;}
        }

        /// <summary>
        /// INitialises an instance of the <see cref="ResourceLoader"/> class.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="assembly"></param>
        public ResourceLoader(string baseName, Assembly assembly)
        {
            m_Manager = new ResourceManager(baseName, assembly);
        }
    }

}
