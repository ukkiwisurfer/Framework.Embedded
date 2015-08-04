
namespace Ignite.Infrastructure.Micro.Common.Services
{
    using System;
    using System.IO;
    using System.Threading;

    using Ignite.Infrastructure.Micro.Common.Assertions;
    using Ignite.Infrastructure.Micro.Common.IO.FileManagement;
    using Ignite.Infrastructure.Micro.Common.IO.Networking;

    /// <summary>
    /// Transfers data that has been persisted locally to a remote machine.
    /// </summary>
    /// <remarks>
    /// Finds any unsent local data files and sends them via a remote message 
    /// broker for processing.
    /// </remarks>
    public class DataTransferService : ThreadedService
    {
        private readonly IMessageBrokerClient m_Client;
        private readonly int m_BufferSize;
        private readonly string m_Path;
        private readonly string m_Extension;
        private readonly IFileHelper m_FileHelper;
        private readonly object m_SyncObject;
        private bool m_IsOpen;

        private int m_MessageBatchSize;
        /// <summary>
        /// The number of log messages to batch up before attempting to send.
        /// </summary>
        public int MessageBatchSize
        {
            get
            {
                lock (m_SyncObject)
                {
                    return m_MessageBatchSize;
                }
            }
            set
            {
                lock (m_SyncObject)
                {
                    m_MessageBatchSize = value;
                }
            }
        }

        /// <summary>
        /// Initialises an instance of the <see cref="DataTransferService"/> class.
        /// </summary>
        /// <param name="client">
        /// The client to use for sending messages to a message broker.
        /// </param>
        /// <param name="fileHelper">
        /// Helper for working with files.
        /// </param>
        /// <param name="path">
        /// The path to query for files to send
        /// </param>
        /// <param name="extension">
        /// The file extension to query for.
        /// </param>
        /// <param name="bufferSize">
        /// The size of the read buffer to use when loading data files.
        /// </param>
        public DataTransferService(IMessageBrokerClient client, IFileHelper fileHelper, string path, string extension, int bufferSize = 1024) : base()
        {
            client.ShouldNotBeNull();
            fileHelper.ShouldNotBeNull();
            path.ShouldNotBeEmpty();
            extension.ShouldNotBeEmpty();

            ServiceName = "DataTransferService";

            m_FileHelper = fileHelper;
            m_SyncObject = new object();
            m_Client = client;
            m_Path = path;
            m_Extension = extension;
            m_BufferSize = bufferSize;

            m_MessageBatchSize = 2;
        }

        /// <summary>
        /// See <see cref="ThreadedService.CheckIfWorkExists"/> for more details.
        /// </summary>
        /// <param name="hasWork"></param>
        public override void CheckIfWorkExists(bool hasWork = false)
        {
            m_FileHelper.CreateDirectory(m_Path);

            var files = m_FileHelper.GetAllFilesMatchingPattern(m_Path, m_Extension);
            if (files.Length > 0)
            {
                this.SignalWorkToBeDone();
            }
        }

        /// <summary>
        /// On attempting to open the service, initialises the connection to the real logging service.
        /// </summary>
        protected override void OnOpening()
        {
            m_IsOpen = true;
        }

        /// <summary>
        /// On attempting to close the service, flushes any pending log messages 
        /// </summary>
        protected override void OnClosing()
        {
            m_IsOpen = false;
        }

        /// <summary>
        /// Perform transfer of log messages to the real logging service.
        /// </summary>
        /// <remarks>
        /// Loads log files off the disk and sends the file contents to a
        /// message broker client. 
        /// </remarks>
        protected override void DoWork()
        {
            try
            {
                m_Client.Open();

                if (m_Client.IsOpen)
                {
                    var pathExists = m_FileHelper.DoesDirectoryExist(m_Path);
                    if (pathExists)
                    {
                        var fileNames = m_FileHelper.GetAllFilesMatchingPattern(m_Path, m_Extension);
                        if (fileNames.Length > 0)
                        {
                            foreach (var fileName in fileNames)
                            {
                                // Open file and read payload
                                using (var file = m_FileHelper.OpenStream(m_Path, fileName, m_BufferSize))
                                {
                                    using (var reader = new StreamReader(file))
                                    {
                                        // Send payload via message broker client.
                                        var payload = reader.ReadToEnd();
                                        m_Client.SendMessages(new object[] { payload });
                                    }
                                }

                                m_FileHelper.DeleteFile(m_Path, fileName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.LogFatal("Error occured trying to transfer data to remote endpoint.", ex);
            }
            finally
            {
                m_Client.Close();
            }
        }

        /// <summary>
        /// Indicates whether the proxy service is available for processing.
        /// </summary>
        public override bool IsServiceActive
        {
            get { return m_IsOpen; }
        }
    }
}
