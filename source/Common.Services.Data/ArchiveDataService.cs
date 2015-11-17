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
using Ignite.Framework.Micro.Common.Assertions;
using Ignite.Framework.Micro.Common.Contract.FileManagement;
using Ignite.Framework.Micro.Common.Contract.Logging;
using Ignite.Framework.Micro.Common.Contract.Services;
using Ignite.Framework.Micro.Common.Core.Extensions;

namespace Ignite.Framework.Micro.Common.Services.Data
{
    /// <summary>
    /// Handles archived data. 
    /// </summary>
    public class ArchiveDataService : ThreadedService
    {
        private readonly BufferedConfiguration m_Configuration;
        private readonly ILogger m_Logger;
        private readonly IFileHelper m_FileHelper;
        private readonly object m_SyncObject;

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

        /// <summary>
        /// Initialises an instance of the <see cref="ArchiveDataService"/> class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="fileHelper"></param>
        /// <param name="configuration"></param>
        public ArchiveDataService(IFileHelper fileHelper, BufferedConfiguration configuration) : base(typeof(ArchiveDataService))
        {
            fileHelper.ShouldNotBeNull();
            configuration.ShouldNotBeNull();

            m_FileHelper = fileHelper;
            m_Configuration = configuration;
            m_SyncObject = new object();
        }

        /// <summary>
        /// Initialises an instance of the <see cref="ArchiveDataService"/> class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="fileHelper"></param>
        /// <param name="configuration"></param>
        public ArchiveDataService(ILogger logger, IFileHelper fileHelper, BufferedConfiguration configuration) : base(logger, typeof(ArchiveDataService))
        {
            logger.ShouldNotBeNull();
            fileHelper.ShouldNotBeNull();
            configuration.ShouldNotBeNull();

            m_FileHelper = fileHelper;
            m_Configuration = configuration;
            m_Logger = logger;
            m_SyncObject = new object();
        }

        /// <summary>
        /// See <see cref="ThreadedService.CheckIfWorkExists"/> for more details.
        /// </summary>
        /// <param name="hasWork"></param>
        public override void CheckIfWorkExists(bool hasWork = false)
        {
            var files = m_FileHelper.GetAllFilesMatchingPattern(m_Configuration.ArchivePath, m_Configuration.ArchiveFileExtension, m_TransferLimit);
            if (files.Count() > 0)
            {
                this.SignalWorkToBeDone();
            }
        }

        /// <summary>
        /// Manages previously archived data.
        /// </summary>
        protected override void DoWork()
        {
            m_Logger.Debug("Started processing.");

            try
            {
                var fileNames = m_FileHelper.GetAllFilesMatchingPattern(m_Configuration.ArchivePath, m_Configuration.ArchiveFileExtension, m_TransferLimit);
                var fileCount = fileNames.Count();

                m_Logger.Debug("Found {0} files.", fileCount);

                if (fileCount > 0)
                {
                    var iterator = fileNames.GetEnumerator();
                    var isValid = iterator.MoveNext();

                    m_Logger.Debug("Deleting {0} files.", fileCount);
                    for (int fileIndex = 0; fileIndex < fileCount; fileIndex++)
                    {
                        if (!isValid) break;

                        var fileName = iterator.Current as string;
                        if (fileName != null)
                        {
                            m_Logger.Debug("Deleting {0} of {1} files.", fileIndex+1, fileCount);

                            long fileSize = m_FileHelper.GetFileSize(m_Configuration.ArchivePath, fileName);
                            if (fileSize > 0)
                            {
                                m_FileHelper.DeleteFile(m_Configuration.ArchivePath, fileName);
                            }
                            else
                            {
                                m_FileHelper.DeleteFile(m_Configuration.ArchivePath, fileName);                                
                            }
                        }

                        isValid = iterator.MoveNext();
                    }
                }
            }
            finally
            {
                this.SignalWorkCompleted();
                m_Logger.Debug("Finished processing.");    
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsServiceActive
        {
            get { return true; }
        }
    }
}
