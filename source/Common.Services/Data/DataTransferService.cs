﻿
namespace Ignite.Framework.Micro.Common.Services.Data
{
    using System;
    using System.IO;

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Messaging;
    using Ignite.Framework.Micro.Common.Contract.Services;
    using Ignite.Framework.Micro.Common.FileManagement;

    /// <summary>
    /// Transfers data that has been persisted locally to a remote machine via pub/sub mechanism.
    /// </summary>
    /// <remarks>
    /// Finds any unsent local data files and sends them to a message 
    /// broker for processing.
    /// </remarks>
    public class DataTransferService : ThreadedService, IBatchConfiguration
    {
        private readonly IMessagePublisher m_Publisher;
        private readonly BufferedConfiguration m_Configuration;
        private readonly IFileHelper m_FileHelper;
        private readonly object m_SyncObject;
        private readonly int m_BufferSize;
        private bool m_IsOpen;

        private int m_MessageBatchSize;
        /// <summary>
        /// The number of files to batch up before attempting to send.
        /// </summary>
        public int BatchSize
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
        /// <param name="publisher">
        /// The client to use for sending messages to a message broker.
        /// </param>
        /// <param name="fileHelper">
        /// Helper for working with files.
        /// </param>
        /// <param name="configuration">
        /// The configuration parameters for where the files are located.
        /// </param>
        /// <param name="bufferSize">
        /// The size of the read buffer to use when loading each data file's contents.
        /// </param>
        public DataTransferService(IMessagePublisher publisher, IFileHelper fileHelper, BufferedConfiguration configuration, int bufferSize = 1024) : base()
        {
            publisher.ShouldNotBeNull();
            fileHelper.ShouldNotBeNull();
            configuration.ShouldNotBeNull();

            ServiceName = "DataTransferService";

            m_FileHelper = fileHelper;
            m_SyncObject = new object();
            m_Publisher = publisher;
            m_BufferSize = bufferSize;
            m_Configuration = configuration;

            m_MessageBatchSize = 5;
        }

        /// <summary>
        /// See <see cref="ThreadedService.CheckIfWorkExists"/> for more details.
        /// </summary>
        /// <param name="hasWork"></param>
        public override void CheckIfWorkExists(bool hasWork = false)
        {
            var files = m_FileHelper.GetAllFilesMatchingPattern(m_Configuration.TargetPath, m_Configuration.TargetFileExtension);
            if (files.Length >= BatchSize)
            {
                this.SignalWorkToBeDone();
            }
        }

        /// <summary>
        /// On attempting to open the service, initialises the connection to the real logging service.
        /// </summary>
        protected override void OnOpening()
        {
            
            m_FileHelper.CreateDirectory(m_Configuration.TargetPath);
            m_Publisher.Connect();

            m_IsOpen = true;
        }

        /// <summary>
        /// On attempting to close the service, flushes any pending log messages 
        /// </summary>
        protected override void OnClosing()
        {
            m_Publisher.Disconnect();
            m_IsOpen = false;
        }

        /// <summary>
        /// Perform transfer of files to a destination service via a message publisher.
        /// </summary>
        /// <remarks>
        /// Loads files off the disk and sends the file contents to a message broker. 
        /// </remarks>
        protected override void DoWork()
        {
            try
            {
                if (m_Publisher.IsConnected)
                {
                    var pathExists = m_FileHelper.DoesDirectoryExist(m_Configuration.TargetPath);
                    if (pathExists)
                    {
                        // Find all files under the target path and with the specified file extension.
                        var fileNames = m_FileHelper.GetAllFilesMatchingPattern(m_Configuration.TargetPath, m_Configuration.TargetFileExtension);
                        if (fileNames.Length > 0)
                        {
                            // For each file, read it and publish its contents via a message broker.
                            foreach (var fileName in fileNames)
                            {
                                // Open file and read payload.
                                using (var fileStream = m_FileHelper.OpenStream(m_Configuration.TargetPath, fileName, m_BufferSize))
                                {
                                    using (var payloadStream = new MemoryStream())
                                    {
                                        // Copy file contents into memory stream.
                                        byte[] buffer = new byte[m_BufferSize];
                                        int bytesRead = 0;

                                        // While there is data to read from the file add it to the payload stream.
                                        while ((bytesRead = fileStream.Read(buffer, 0, m_BufferSize)) > 0)
                                        {
                                            payloadStream.Write(buffer, 0, bytesRead);
                                        }

                                        // Publish message with file contents.
                                        m_Publisher.Publish(payloadStream.ToArray());
                                    }
                                }

                                // Once sent, delete the file.
                                m_FileHelper.DeleteFile(m_Configuration.TargetPath, fileName);
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
            }
        }

        /// <summary>
        /// Indicates whether the proxy service is available for processing.
        /// </summary>
        public override bool IsServiceActive
        {
            get { return m_Publisher.IsConnected; }
        }
    }
}