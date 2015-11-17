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

using System.Threading;
using Microsoft.SPOT;

namespace Ignite.Framework.Micro.Common.Services.Logging
{
    using System;
    using System.IO;
    using System.Collections;
    using System.Text;

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Logging;
    using Ignite.Framework.Micro.Common.Contract.Services;
    using Ignite.Framework.Micro.Common.Contract.FileManagement;
    using Ignite.Framework.Micro.Common.Services.Data;
    using Ignite.Framework.Micro.Common.Logging;

    /// <summary>
    /// Captures logging requests and buffers them before writing them to disk.
    /// </summary>
    /// <remarks>
    /// The batch size determines how many entries are buffered before being written
    /// to disk.
    /// </remarks>
    public class BufferedLoggingService : ThreadedService
    {
        protected readonly IFileHelper m_FileHelper;
        private readonly string m_WorkingFilePath;
        private readonly string m_TargetFilePath;
        private readonly string m_TargetFileExtension;
        private readonly string m_WorkingFileExtension;
        private readonly LogContainer m_LogEntries;
        private int m_BufferSize;
        private int m_BatchSize;

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
                return m_BufferSize;
            }
            set
            {
                m_BufferSize = value;                
            }
        }

        /// <summary>
        /// The number of log entries to cache in memory before persisting the entries.
        /// </summary>
        public int BatchSize
        {
            get
            {
                return m_BatchSize;
            }
            set
            {
                 m_BatchSize = value;
            }
        }

        /// <summary>
        /// Initialises an instance of the <see cref="BufferedLoggingService"/> class. 
        /// </summary>
        /// <param name="fileHelper">
        /// Helper for working with files.
        /// </param>
        /// <param name="configuration">
        /// Configuration details for buffered data persistence. 
        /// </param>
        public BufferedLoggingService(LogContainer container, ILogger logger, IFileHelper fileHelper, BufferedConfiguration configuration) : base(logger, typeof(BufferedLoggingService))
        {
            logger.ShouldNotBeNull();
            fileHelper.ShouldNotBeNull();
            configuration.ShouldNotBeNull();

            m_LogEntries = container;
            m_BatchSize = 2;
            m_BufferSize = 512;

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
        /// Adds a new logging entry to the proxy service.
        /// </summary>
        /// <remarks>
        /// Will add a new log entry in a thread safe manner.
        /// </remarks>
        /// <param name="logEntry">
        /// The log entry to add.
        /// </param>
        public void AddLogEntry(LogEntry logEntry)
        {
            logEntry.ShouldNotBeNull();

            var count = m_LogEntries.AddLogEntry(logEntry);
            if (count >= m_BatchSize)
            {
                SignalWorkToBeDone();
            }
        }

        /// <summary>
        /// Checks for messages to process.
        /// </summary>
        /// <param name="signalled"></param>
        public override void CheckIfWorkExists(bool hasWork)
        {
            if (m_LogEntries.Count >= m_BatchSize)
            {
                SignalWorkToBeDone();
            }
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

            try
            {
                // Lock only long enough dequeue items from the queue.
                for (var index = 0; index < m_BatchSize; index++)
                {
                    var logEntry = m_LogEntries.GetNextEntry();
                    if (logEntry != null)
                    {
                        messages.Add(logEntry);
                    }
                }

                // Sends the logged messages.
                if (messages.Count > 0)
                {
                    LogMessagesToConsole(messages);
                }
            }
            finally
            {
                SignalWorkCompleted();
                LogDebug("Processing completed.");
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
            m_FileHelper.CreateDirectory(m_WorkingFilePath);
            m_FileHelper.CreateDirectory(m_TargetFilePath);
        }

        /// <summary>
        /// See <see cref="ThreadedService.OnClosing"/> for more details.
        /// </summary>
        protected override void OnClosing()
        {
        }

        /// <summary>
        /// Logs messages to the console window.
        /// </summary>
        /// <param name="dataItems"></param>
        protected virtual void LogMessagesToConsole(ArrayList dataItems)
        {
            var formatter = new NLogFormatter();
            foreach (var item in dataItems)
            {
                var dataItem = item as LogEntry;
                if (dataItem != null)
                {
                    formatter.Log(dataItem);
                }
            }
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
