
namespace Ignite.Infrastructure.Micro.Contract.Services
{
    /// <summary>
    /// Configuration information for received data that is being buffered.
    /// </summary>
    public class BufferedConfiguration
    {
        /// <summary>
        /// The path where working files are stored.
        /// </summary>
        public string WorkingPath { get; set; }

        /// <summary>
        /// The file extension that working files are suffixed with.
        /// </summary>
        public string WorkingFileExtension { get; set; }

        /// <summary>
        /// The path where completed data files are stored.
        /// </summary>
        public string TargetPath { get; set; }

        /// <summary>
        /// The file extension that completed data files are suffuxed with.
        /// </summary>
        public string TargetFileExtension { get; set; }
    }
}
