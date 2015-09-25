namespace Ignite.Framework.Micro.Common.Bootstrap
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Loads the bootstrap loader from the SD card.
    /// </summary>
    public class BootstrapLoader
    {
        /// <summary>
        /// Initialises an instance of the bootstrap loader.
        /// </summary>
        public BootstrapLoader()
        {
        }

        /// <summary>
        /// Loads the bytecode for an assembly off disk.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="appDomain"></param>
        /// <returns></returns>
        public Assembly LoadFromFile(string filePath, AppDomain appDomain)
        {

            try
            {
                using (var filestream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    int fileLength = (int)filestream.Length;
                    byte[] bytecode = new byte[fileLength];

                    filestream.Read(bytecode, 0, fileLength);

                    return Assembly.Load(bytecode);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
