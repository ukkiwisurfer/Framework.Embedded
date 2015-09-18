
namespace Ignite.Framework.Micro.Common.Services.Data
{
    using System;
    using System.Collections;
    using System.IO;
    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Services;
    using Ignite.Framework.Micro.Common.FileManagement;

    /// <summary>
    /// Captures data packets and buffers them before writing them to disk.
    /// </summary>
    /// <remarks>
    /// The batch size determines how many are packets are buffered before 
    /// being written to disk.
    /// </remarks>
    public abstract class BufferedDataService : ThreadedService, IBufferConfiguration, IBatchConfiguration
    {
        private readonly IFileHelper m_FileHelper;
        private readonly BufferedConfiguration m_Configuration;
        private readonly string m_WorkingFilePath;
        private readonly string m_TargetFilePath;
        private readonly string m_TargetFileExtension;
        private readonly string m_WorkingFileExtension;
        private int m_BatchSize;
        private int m_BufferSize;

        private readonly object m_SyncLock;
        /// <summary>
        /// Synchronisation object to use for shared access resources.
        /// </summary>
        protected object SyncLock
        {
            get { return m_SyncLock; }
        }

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
        /// <param name="configuration">
        /// Configuration details for buffered data persistence. 
        /// </param>
        /// <param name="fileHelper">
        /// Helper for working with files.
        /// </param>
        protected BufferedDataService(IFileHelper fileHelper, BufferedConfiguration configuration)
        {
            fileHelper.ShouldNotBeNull();
            configuration.ShouldNotBeNull();

            m_MessageQueue = new Queue();
            m_SyncLock = new object();
            m_BatchSize = 2;
            m_BufferSize = 1024;

            m_FileHelper = fileHelper;
            m_Configuration = configuration;
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
        /// Opens new file stream
        /// </summary>
        /// <param name="filePath">
        /// The fully qualified path where the files are to be written to. 
        /// </param>
        /// <param name="fileName">
        /// The name of the file to open or create.
        /// </param>
        /// <returns>
        /// A stream representing the opened file.
        /// </returns>
        protected virtual Stream OpenStream(string filePath, string fileName)
        {
            return this.m_FileHelper.OpenStream(filePath, fileName);
        }

        /// <summary>
        /// Returns the stream used to write logging entries to.
        /// </summary>
        /// <remarks>
        /// If the new file doesn't exist we have just crossed a transition boundary.
        /// Rename any files that end in .raw to .log.
        /// <para></para>
        /// This indicates that the file can now safely be processed. This supports
        /// the scenario where a raw file can be amended multiple times - in which case
        /// we do not want the log transfer service to process the file (as we might
        /// be potentially writing to it at the same time).
        /// </remarks>
        /// <returns>
        /// A stream writer used to write logging entries to.
        /// </returns>
        protected StreamWriter GetFileStream(string sourceFilePath, string targetFilePath, string fileName = null)
        {
            StreamWriter writer = null;

            string generatedFileName = fileName ?? this.m_FileHelper.GenerateFileName(DateTime.Now, m_WorkingFileExtension);
            string pathAndfileName = this.m_FileHelper.BuildFilePath(sourceFilePath, generatedFileName);

            try
            {
                if (!File.Exists(pathAndfileName))
                {
                    this.m_FileHelper.RenameAllFilesMatchingExtension(sourceFilePath, targetFilePath, m_WorkingFileExtension, m_TargetFileExtension);
                }

                var fileStream = this.m_FileHelper.OpenStream(sourceFilePath, generatedFileName);
                fileStream.Seek(0, SeekOrigin.End);

                writer = new StreamWriter(fileStream);
            }
            catch (Exception ex)
            {                
            }

            return writer;
        }

        /// <summary>
        /// See <see cref="ThreadedService.DoWork"/> for more details.
        /// </summary>
        /// <remarks>
        /// Main processing logic for when work is detected.
        /// </remarks>
        protected override void DoWork()
        {
            object[] messages = null;

            // Lock only long enough to drain the queue and copy the messages.
            lock (m_SyncLock)
            {
                if (m_MessageQueue.Count > 0)
                {
                    messages = m_MessageQueue.ToArray();
                    m_MessageQueue.Clear();
                }

                this.SignalWorkCompleted();
            }

            // Sends the logged messages.
            if (messages != null)
            {
                this.WriteData(messages);
            }   
        }

        /// <summary>
        /// Adds a new data item.
        /// </summary>
        /// <remarks>
        /// Will add a new data item in a thread safe manner.
        /// </remarks>
        /// <param name="dataItem">
        /// The data item to add.
        /// </param>
        protected void AddDataItem(object dataItem)
        {
            dataItem.ShouldNotBeNull();

            lock (SyncLock)
            {
                MessageQueue.Enqueue(dataItem);

                if (MessageQueue.Count >= BatchSize)
                {
                    this.SignalWorkToBeDone();
                }
            }
        }

        /// <summary>
        /// Provides a stub method to override to support the persistence of messages
        /// to disk.
        /// </summary>
        /// <param name="dataItems">
        /// A collection of data items 
        /// </param>
        protected abstract void WriteData(object[] dataItems);

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
        /// See <see cref="ThreadedService.IsServiceActive"/> for more details.
        /// </summary>
        public override bool IsServiceActive
        {
            get { return true; }
        }
    }
}