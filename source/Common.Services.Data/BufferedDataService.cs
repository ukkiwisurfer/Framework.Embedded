//--------------------------------------------------------------------------- 
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

using Ignite.Framework.Micro.Common.Core;
using Ignite.Framework.Micro.Common.Data;
using Ignite.Framework.Micro.Common.Logging;
using Microsoft.SPOT;

namespace Ignite.Framework.Micro.Common.Services.Data
{
    using System;
    using System.Collections;
    using System.IO;

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Logging;
    using Ignite.Framework.Micro.Common.Contract.Services;
    using Ignite.Framework.Micro.Common.Contract.FileManagement;

    /// <summary>
    /// Captures data packets and buffers them before writing them to disk.
    /// </summary>
    /// <remarks>
    /// The batch size determines how many are packets are buffered before 
    /// being written to disk.
    /// </remarks>
    public class BufferedDataService : ThreadedService, IBufferConfiguration, IBatchConfiguration
    {
        protected readonly IFileHelper m_FileHelper;
        private readonly string m_WorkingFilePath;
        private readonly string m_TargetFilePath;
        private readonly string m_TargetFileExtension;
        private readonly string m_WorkingFileExtension;
        private readonly string m_IPAddress;
        private int m_BatchSize;
        private int m_BufferSize;
        private readonly object m_SyncLock;

        private readonly Queue m_MessageQueue;
        /// <summary>
        /// Received data items that have been queued for processing. 
        /// </summary>
        protected Queue MessageQueue
        {
            get { return m_MessageQueue; }
        }

        /// <summary>
        /// Path where working data files will be written to.
        /// </summary>
        protected string WorkingPath
        {
            get { return m_WorkingFilePath; }
        }

        /// <summary>
        /// Path where completed data files will be written to.
        /// </summary>
        protected string TargetPath
        {
            get { return m_TargetFilePath; }
        }

        /// <summary>
        /// Indicates the size of the buffer to use when writing data.
        /// </summary>
        public int BufferSizeInBytes
        {
            get
            {
                lock (m_SyncLock)
                {
                    return m_BufferSize;
                }
            }
            set
            {
                lock(m_SyncLock)
                {
                    m_BufferSize = value;
                }
            }
        }

        /// <summary>
        /// The number of log entries to cache in memory before persisting the entries.
        /// </summary>
        public int BatchSize
        {
            get
            {
                lock (m_SyncLock)
                {
                    return m_BatchSize;
                }
            }
            set
            {
                lock (m_SyncLock)
                {
                    m_BatchSize = value;
                }
            }
        }

        /// <summary>
        /// Initialises an instance of the <see cref="BufferedDataService"/> class. 
        /// </summary>
        /// <param name="logger">
        /// Logging provider.
        /// </param>
        /// <param name="fileHelper">
        /// Helper for working with files.
        /// </param>
        /// <param name="configuration">
        /// Configuration parameters for persisting data (buffered data). 
        /// </param>
        public BufferedDataService(ILogger logger, IFileHelper fileHelper, BufferedConfiguration configuration) : base(logger, typeof(BufferedDataService))
        {
            logger.ShouldNotBeNull();
            fileHelper.ShouldNotBeNull();
            configuration.ShouldNotBeNull();

            m_MessageQueue = new Queue();
            m_SyncLock = new object();
            m_BatchSize = 2;
            m_BufferSize = 1024;

            m_IPAddress = "192.168.1.105";
            m_FileHelper = fileHelper;
            m_WorkingFilePath = configuration.WorkingPath;
            m_TargetFilePath = configuration.TargetPath;
            m_TargetFileExtension = configuration.TargetFileExtension;
            m_WorkingFileExtension = configuration.WorkingFileExtension;
        }

        /// <summary>
        /// See <see cref="IDisposable.Dispose"/> for more details.
        /// </summary>
        /// <param name="isDisposing">
        /// Indicates whether the dispose is deterministic.
        /// </param>
        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
            }

 	        base.Dispose(isDisposing);
        }

        /// <summary>
        /// See <see cref="ThreadedService.DoWork"/> for more details.
        /// </summary>
        /// <remarks>
        /// Main processing logic for when work is detected. Looks for messages and if there are any, dequeues them
        /// and writes them to the file system.
        /// <para></para>
        /// There is a possibility that the messages in the memory queue are lost if the process is restarted before
        /// the messages are dequeued and persisted. That trade-off can be mitigated by reudcing the value of the 
        /// BatchSize property. It also means a corresponding increase in latency when processing incoming messages 
        /// in order to persist the messages to the underlying filesystem.
        /// <para></para>
        /// Given the environment is a memory constrained one, at this point 
        /// the trade-off is deemed acceptible for the current requirements of how the framework will be used.
        /// </remarks>
        protected override void DoWork()
        {
            var messages = new ArrayList();

            LogDebug("Started processing.");

            var logger = new ConsoleLogger();
            logger.LogFreeMemory();

            try
            {
                // Find out how many items are in the queue.
                int messageCount = 0;
                lock (m_SyncLock)
                {
                    messageCount = m_MessageQueue.Count;
                }

                // Only dequeue if the batch size has been reached.
                if (messageCount >= m_BatchSize)
                {
                    for (var index = 0; index < m_BatchSize; index++)
                    {
                        lock (m_SyncLock)
                        {
                            messages.Add(m_MessageQueue.Dequeue());
                        }
                    }
                }

                // Sends the logged messages.
                if (messages.Count > 0)
                {
                    WriteData(messages);
                }

                logger.LogFreeMemory();
            }
            finally
            {
                this.SignalWorkCompleted();
                LogDebug("Processing completed.");

            }
        }

        /// <summary>
        /// Adds a new data item.
        /// </summary>
        /// <remarks>
        /// Will add a new data item in a thread safe manner. If the number of items in the memory
        /// queue is larger or equal to the BatchSize property, the service will signal that the
        /// messages need to be persisted.
        /// </remarks>
        /// <param name="dataItem">
        /// The data item to add.
        /// </param>
        public void AddDataItem(DataItem dataItem)
        {
            dataItem.ShouldNotBeNull();

            try
            {
                int messageCount = 0;
                lock (m_SyncLock)
                {
                    MessageQueue.Enqueue(dataItem);
                    messageCount = MessageQueue.Count;
                }

                if (messageCount >= BatchSize)
                {
                    this.SignalWorkToBeDone();
                }
            }
            catch (Exception ex)
            {
                LogError("Exception detected adding data item. Stacktrace: {0}", ex, ex.StackTrace);
            }
        }

        /// <summary>
        /// Provides a method to override to support the persistence of messages to disk.
        /// </summary>
        /// <remarks>
        /// Expects source files to be in an separate directory with a different extension.
        /// In order for files to be processed, they must be renamed and moved to the target
        /// directory.
        /// </remarks>
        /// <param name="dataItems">
        /// The collection of items 
        /// </param>
        protected virtual void WriteData(ArrayList dataItems)
        {
            LogDebug("{0} data items to be written.", dataItems.Count);

            foreach (var item in dataItems)
            {
                var dataItem = item as DataItem;
                if (dataItem != null)
                {
                    try
                    {
                        string generatedFileName = m_FileHelper.GenerateFileName(DateTime.Now,  m_WorkingFileExtension);

                        using (var stream = m_FileHelper.OpenStreamForWrite(m_WorkingFilePath, generatedFileName, 0))
                        {
                            if (stream != null)
                            {
                                using (var builder = new DataStreamBuilder(stream))
                                {
                                    builder.SetMetadata(m_IPAddress, dataItem.CaptureTimestamp);
                                    builder.SetPayload(dataItem.Payload);
                                }
                            }
                        }

                        m_FileHelper.Flush();

                        LogDebug("File created. FilePath: '{0}', Filename: '{1}'", m_WorkingFilePath,  generatedFileName);
                        m_FileHelper.MoveFile(m_WorkingFilePath, generatedFileName, m_TargetFilePath,  m_TargetFileExtension);
                    }
                    catch (Exception ex)
                    {
                        LogError("Exception raised while writing data items. Stacktrace: {0}.", ex, ex.StackTrace);
                    }
                }

            }
        }

        /// <summary>
        /// See <see cref="ThreadedService.OnOpening"/> for more details.
        /// </summary>
        /// <remarks>
        /// Creates the required directories if they do not already exist.
        /// </remarks>
        protected override void OnOpening()
        {
            base.OnOpening();

            m_FileHelper.CreateDirectory(m_WorkingFilePath);
            m_FileHelper.CreateDirectory(m_TargetFilePath);                
        }

        /// <summary>
        /// See <see cref="ThreadedService.IsServiceActive"/> for more details.
        /// </summary>
        public override bool IsServiceActive
        {
            get { return true; }
        }
    }
}
