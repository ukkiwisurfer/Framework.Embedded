﻿//--------------------------------------------------------------------------- 
//   Copyright 2014-2015 Igniteous Limited
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
//----------------------------------------------------------------------------- 

using System.Text;
using Microsoft.SPOT;

namespace Ignite.Framework.Micro.Common.Services.Data
{
    using System;
    using System.Threading;
    using System.IO;

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Logging;
    using Ignite.Framework.Micro.Common.Contract.Messaging;
    using Ignite.Framework.Micro.Common.Contract.Services;
    using Ignite.Framework.Micro.Common.Contract.FileManagement;
    using Ignite.Framework.Micro.Common.Core.Extensions;
    using Ignite.Framework.Micro.Common.Contract.Hardware;

    /// <summary>
    /// Transfers data that has been persisted locally to a remote machine via pub/sub mechanism.
    /// </summary>
    /// <remarks>
    /// Finds any unsent local data files and sends them to a message 
    /// broker for processing.
    /// </remarks>
    public class DataTransferService : ThreadedService
    {
        private readonly IResourceLoader m_ResourceLoader;
        private readonly IMessagePublisher m_Publisher;
        private readonly BufferedConfiguration m_Configuration;
        private readonly IFileHelper m_FileHelper;
        private readonly ILed m_Led;
        private readonly object m_SyncObject;
        private readonly int m_BufferSize;
        private int m_Semaphore;

        private bool m_IsOpen;

        private int m_TransferLimit;
        /// <summary>
        /// The maximum number of files to transfer in one call.
        /// </summary>
        public int TransferLimit
        {
            get
            {
                lock (m_SyncObject)
                {
                    return m_TransferLimit;
                }
            }
            set
            {
                lock (m_SyncObject)
                {
                    m_TransferLimit = value;
                }
            }
        }

        private int m_PulsePeriodInMilliseconds;
        /// <summary>
        /// The period that the LED should pulse for to indicate life.
        /// </summary>
        public int PulsePeriodInMilliseconds
        {
            get
            {
                lock (m_SyncObject)
                {
                    return m_PulsePeriodInMilliseconds;
                }
            }
            set
            {
                lock (m_SyncObject)
                {
                    m_PulsePeriodInMilliseconds = value;
                }
            }
        }

        private int m_MaximumFileSizeInBytes;
        /// <summary>
        /// The maximum file size in bytes.
        /// </summary>
        public int MaximmumFileSizeInBytes
        {
            get
            {
                lock (m_SyncObject)
                {
                    return m_MaximumFileSizeInBytes;
                }
            }
            set
            {
                lock (m_SyncObject)
                {
                    m_MaximumFileSizeInBytes = value;
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
        public DataTransferService(IMessagePublisher publisher, IFileHelper fileHelper, ILed led, BufferedConfiguration configuration, int bufferSize = 512)
            : base(typeof(DataTransferService))
        {
            publisher.ShouldNotBeNull();
            fileHelper.ShouldNotBeNull();
            configuration.ShouldNotBeNull();

            m_ResourceLoader = new ServicesResourceLoader();
            m_FileHelper = fileHelper;
            m_SyncObject = new object();
            m_Publisher = publisher;
            m_BufferSize = bufferSize;
            m_Configuration = configuration;
            m_Led = led;
            m_PulsePeriodInMilliseconds = 1000;

            m_TransferLimit = 5;
            m_MaximumFileSizeInBytes = 1500;
        }

        /// <summary>
        /// Initialises an instance of the <see cref="DataTransferService"/> class.
        /// </summary>
        /// <param name="logger"></param>
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
        public DataTransferService(ILogger logger, IMessagePublisher publisher, IFileHelper fileHelper, ILed led, BufferedConfiguration configuration, int bufferSize = 400)
            : base(logger, typeof(DataTransferService))
        {
            publisher.ShouldNotBeNull();
            fileHelper.ShouldNotBeNull();
            configuration.ShouldNotBeNull();
            led.ShouldNotBeNull();

            m_ResourceLoader = new ServicesResourceLoader();
            m_FileHelper = fileHelper;
            m_SyncObject = new object();
            m_Publisher = publisher;
            m_BufferSize = bufferSize;
            m_Configuration = configuration;
            m_Led = led;

            m_TransferLimit = 5;
            m_MaximumFileSizeInBytes = 1500;
        }

        /// <summary>
        /// See <see cref="ThreadedService.CheckIfWorkExists"/> for more details.
        /// </summary>
        /// <param name="hasWork"></param>
        public override void CheckIfWorkExists(bool hasWork = false)
        {
            var files = m_FileHelper.GetAllFilesMatchingPattern(m_Configuration.TargetPath, m_Configuration.TargetFileExtension, m_TransferLimit);
            if (files.Count() > 0)
            {
                this.SignalWorkToBeDone();
            }
        }

        /// <summary>
        /// On attempting to open the service, initialises the connection to the real logging service.
        /// </summary>
        protected override void OnOpening()
        {
            base.OnOpening();

            m_FileHelper.CreateDirectory(m_Configuration.TargetPath);
            m_IsOpen = true;
        }

        /// <summary>
        /// On attempting to close the service, flushes any pending log messages 
        /// </summary>
        protected override void OnClosing()
        {
            try
            {
                m_Publisher.Disconnect();
            }
            finally
            {
                m_IsOpen = false;
            }

            base.OnClosing();
        }

        /// <summary>
        /// Perform transfer of files to a destination service via a message publisher.
        /// </summary>
        /// <remarks>
        /// Loads files off the disk and sends the file contents to a message broker. 
        /// </remarks>
        protected override void DoWork()
        {
            var isRunning = Interlocked.CompareExchange(ref m_Semaphore, 1, 0);

            if (isRunning != 1)
            {
                LogDebug("Starting processing.");
                try
                {
                    var free = Debug.GC(true);
                    LogDebug("Memory free: {0} bytes.", free);

                    m_Led.On();

                    if (!m_Publisher.IsConnected) m_Publisher.Connect();

                    if (m_Publisher.IsConnected)
                    {
                        var pathExists = m_FileHelper.DoesDirectoryExist(m_Configuration.TargetPath);
                        if (pathExists)
                        {
                            // Find all files under the target path and with the specified file extension.
                            var fileNames = m_FileHelper.GetAllFilesMatchingPattern(m_Configuration.TargetPath, m_Configuration.TargetFileExtension, m_TransferLimit);
                            var fileCount = fileNames.Count();
                            if (fileCount > 0)
                            {
                                var iterator = fileNames.GetEnumerator();
                                var isValid = iterator.MoveNext();



                                // For each file, read it and publish its contents via a message broker.
                                for (int fileIndex = 0; fileIndex < fileCount; fileIndex++)
                                {
                                   if (!isValid) break;

                                    var fileName = iterator.Current as string;
                                    if (fileName != null)
                                    {
                                //        LogDebug("About to commence processing file - FilePath: '{0}', FileName: '{1}'", m_Configuration.TargetPath, fileName);

                                //        long fileSize = m_FileHelper.GetFileSize(m_Configuration.TargetPath, fileName);
                                //        if (fileSize > 0)
                                //        {
                                //            LogDebug("Started processing file - FileSize: '{0}'", fileName, fileSize);

                                //            if (fileSize <= m_MaximumFileSizeInBytes)
                                //            {

                                //                LogDebug("File is within allowable size. FileSize: '{0}'", fileSize);

                                //                string filePath = m_FileHelper.BuildFilePath(m_Configuration.TargetPath, fileName);
                                //                var buffer = File.ReadAllBytes(filePath);

                                //                LogDebug("Publishing memory stream contents. Size: {0}", buffer.Length);
                                //                m_Publisher.Publish(ref buffer);

                                //                try
                                //                {
                                //                    m_FileHelper.MoveFile(m_Configuration.TargetPath, fileName, m_Configuration.ArchivePath, m_Configuration.ArchiveFileExtension);
                                //                }
                                //                catch (Exception ex)
                                //                {
                                //                    LogDebug("Exception detected trying to move file '{0}'. Attempting to delete file.", fileName);
                                //                    m_FileHelper.DeleteFile(m_Configuration.TargetPath, fileName);
                                //                    LogDebug("File deleted. FileName: '{0}'.", fileName);
                                //                }
                                //            }
                                //        }
                                //        else
                                //        {
                                //            // Delete the file.
                                //            m_FileHelper.MoveFile(m_Configuration.TargetPath, fileName, m_Configuration.ArchivePath, m_Configuration.ArchiveFileExtension);
                                //            //m_FileHelper.DeleteFile(m_Configuration.TargetPath, fileName);
                                //        }

                                    }

                                    m_FileHelper.Flush();

                                    LogDebug("Getting next filename from iterator.");
                                    isValid = iterator.MoveNext();
                                }
                            }

                        }

                    }
                }
                catch (Exception ex)
                {
                    this.LogFatal(m_ResourceLoader.GetString(Resources.StringResources.ErrorOccuredWhileTransferringData), ex);
                }
                finally
                {
                    LogDebug("Processing completed.");
                    SignalWorkCompleted();
                    m_Led.Off();

                    var free = Debug.GC(true);
                    LogDebug("Memory free: {0} bytes.", free);

                    Interlocked.CompareExchange(ref m_Semaphore, 0, 1);
                }
            }
        }

        /// <summary>
        /// Indicates whether the proxy service is available for processing.
        /// </summary>
        public override bool IsServiceActive
        {
            get { return m_IsOpen; }
        }

        /// <summary>
        /// On detection of a change in processing state, update the status of the message publisher.
        /// </summary>
        /// <param name="processingState">
        /// The value of the procesing state flag.
        /// </param>
        protected override void OnProcessingStateChanged(bool processingState)
        {
            if (!processingState)
            {
                m_Publisher.Disconnect();
            }
        }
    }
}
