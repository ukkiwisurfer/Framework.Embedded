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

using Microsoft.SPOT.IO;

namespace Ignite.Framework.Micro.Common.FileManagement
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Text;

    using Ignite.Framework.Micro.Common.Contract.Logging;
    using Ignite.Framework.Micro.Common.Core.Extensions;
    using Ignite.Framework.Micro.Common.Contract.FileManagement;

    /// <summary>
    /// Supports file operations.
    /// </summary>
    public class FileSystemHelper : IFileHelper
    {
        private readonly ILogger m_Logger;
        private readonly VolumeInfo m_VolumeInfo;

        /// <summary>
        /// Initialises an instance of the <see cref="FileSystemHelper"/> class.
        /// </summary>
        /// <param name="logger"></param>
        public FileSystemHelper(ILogger logger, VolumeInfo info)
        {
            m_Logger = logger;
            m_VolumeInfo = info;
        }

        /// <summary>
        /// Sets the current directory.
        /// </summary>
        /// <param name="path"></param>
        public void SetCurrentDirectory(string path)
        {
            Directory.SetCurrentDirectory(path);
        }

        /// <summary>
        /// Creates a directory.
        /// </summary>
        /// <param name="path">
        /// The full directory path.
        /// </param>
        public void CreateDirectory(string path)
        {
            var info = new DirectoryInfo(path);
            if (!info.Exists)
            {
                info.Create();
                m_VolumeInfo.FlushAll();
            }
        }

        /// <summary>
        /// Generates the name of the file.
        /// </summary>
        /// <returns>
        /// The name of the file
        /// </returns>
        public string GenerateFileName(DateTime timestamp, string extension)
        {
            var builder = new StringBuilder(timestamp.ToString("yyyyMMddHHmmss"));
            builder.Append(timestamp.Millisecond.ToString("D3"));
            builder.Append(".");
            builder.Append(extension);

            return builder.ToString();
        }

        /// <summary>
        /// Renames an existing file to a new file name.
        /// </summary>
        /// <param name="sourceFileName">
        /// The name of the source file.
        /// </param>
        /// <param name="targetFileName">
        /// The name of the target file.
        /// </param>
        public void RenameFile(string sourceFileName, string targetFileName, bool flush = true)
        {
            if (File.Exists(targetFileName))
            {
                DeleteFile(targetFileName);
            }

            File.Move(sourceFileName, targetFileName);

            if (flush)
            {
                m_VolumeInfo.FlushAll();
            }
        }

        /// <summary>
        /// Renames an existing file to a new file name.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="sourceFileName">
        /// The name of the source file.
        /// </param>
        /// <param name="targetPath">
        /// 
        /// </param>
        /// <param name="targetExtension">
        /// The name of the target file.
        /// </param>
        public void MoveFile(string sourcePath, string sourceFileName, string targetPath, string targetExtension, bool flush = true)
        {
            var source = Path.Combine(sourcePath, sourceFileName);
            var targetFileName = Path.ChangeExtension(sourceFileName, targetExtension);
            var target = Path.Combine(targetPath, targetFileName);

            m_Logger.Debug("Attempting to move file. Source filename: '{0}', Target filename: '{1}'", source, target);

            try
            {
                File.Move(source, target);

                if (flush)
                {
                    m_VolumeInfo.FlushAll();
                }

                var info = new FileInfo(target);
                var isMoved = info.Exists;

                m_Logger.Debug("File move. Source filename: '{0}', Target filename: '{1}', HasMoved: {2}.", source, target, isMoved);
            }
            catch (Exception ex)
            {
                m_Logger.Debug("Exception encountered attempting to move file. Source filename: '{0}', Target filename: '{1}'.", source, target, ex);
            }

        }

        /// <summary>
        /// Returns the size of the file
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public long GetFileSize(string targetPath, string fileName)
        {
            long fileSize = 0;

            string filePath = Path.Combine(targetPath, fileName);

            var info = new FileInfo(filePath);
            if (info.Exists)
            {
                fileSize = info.Length;
                m_Logger.Debug("Interrogated file metadata. Filesize: {0} bytes, ArchivePath: '{1}', Filename: '{2}'", fileSize, targetPath, fileName);
            }
            else
            {
                m_Logger.Debug("File not found. ArchivePath: '{0}', Filename: '{1}'", fileSize, targetPath, fileName);         
            }

            return fileSize;
        }

        /// <summary>
        /// Renames all files ending with the supplied extension to end with '.log' extension.
        /// </summary>
        /// <param name="sourcePath">
        /// The source directory path.
        /// </param>
        /// <param name="targetPath">
        /// The target directory path.
        /// </param>
        /// <param name="oldExtension">
        /// The file extension to rename from.
        /// </param>
        /// <param name="newExtension">
        /// The new file extension to rename to
        /// </param>
        public void RenameAllFilesMatchingExtension(string sourcePath, string targetPath, string oldExtension, string newExtension, bool flush = true)
        {
            try
            {
                var fileNames = Directory.EnumerateFiles(sourcePath);
                var patternCriteria = @"." + oldExtension;

                foreach (string oldFileNameWithPath in fileNames)
                {
                    if (oldFileNameWithPath.LastIndexOf(patternCriteria) > 0)
                    {
                        string oldFileName = Path.GetFileName(oldFileNameWithPath);
                        string newFileName = Path.GetFileNameWithoutExtension(oldFileName) + "." + newExtension;
                        string newFileNameWithPath = this.BuildFilePath(targetPath, newFileName);

                        RenameFile(oldFileNameWithPath, newFileNameWithPath, flush);
                    }
                }

                m_VolumeInfo.FlushAll();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Opens/creates an existing file stream.
        /// </summary>
        /// <param name="filePath">
        /// The path to the file.
        /// </param>
        /// <param name="fileName">
        /// The name of the file.
        /// </param>
        /// <param name="bufferSize">
        /// The size of the read buffer to associate with the file stream.
        /// </param>
        /// <returns></returns>
        public virtual Stream OpenStreamForRead(string filePath, string fileName, int bufferSize = 512)
        {
            var fileNameWithPath = Path.Combine(filePath, fileName);
            return new FileStream(fileNameWithPath, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize);
        }

        /// <summary>
        /// Opens/creates an new file stream.
        /// </summary>
        /// <param name="filePath">
        /// The path to the file.
        /// </param>
        /// <param name="fileName">
        /// The name of the file.
        /// </param>
        /// <param name="bufferSize">
        /// The size of the read buffer to associate with the file stream.
        /// </param>
        /// <returns></returns>
        public virtual Stream OpenStreamForWrite(string filePath, string fileName, int bufferSize = 0)
        {
            var fileNameWithPath = Path.Combine(filePath, fileName);
            return new FileStream(fileNameWithPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize);
        }

        /// <summary>
        /// Returns all files matching the given search pattern for the specified folder.
        /// </summary>
        /// <param name="folder">
        /// The path to search in.
        /// </param>
        /// <param name="pattern">
        /// The file pattern to query for.
        /// </param>
        /// <param name="fileLimit">
        /// The maximum number of files to process per call.
        /// </param>
        /// <returns>
        /// A collection of all filenames matching the search criteria.
        /// </returns>
        public IEnumerable GetAllFilesMatchingPattern(string folder, string pattern, int fileLimit = 5)
        {
            var patternCriteria = @"." + pattern;
            var matches = new ArrayList();

            try
            {
                m_Logger.Debug("Searching for files to process. Folder: '{0}', Pattern: '{1}', FileLimit: {2}.", folder, pattern, fileLimit);

                var fileNames = Directory.EnumerateFiles(folder);
                var fileCount = fileNames.Count();

                m_Logger.Debug("Found {0} files. Limited to processing {1} files per invocation.", fileCount, fileLimit);
                if (fileCount > 0)
                {
                    var iterator = fileNames.GetEnumerator();
                    var isValid = iterator.MoveNext();

                    if (isValid)
                    {
                        var limit = (fileCount < fileLimit) ? fileCount : fileLimit;

                        for (int fileIndex = 0; fileIndex < limit; fileIndex++)
                        {
                            if (!isValid) break;

                            var oldFileNameWithPath = iterator.Current as string;
                            if (oldFileNameWithPath != null)
                            {
                                var foundIndex = oldFileNameWithPath.LastIndexOf(patternCriteria);
                                if (foundIndex > 0)
                                {
                                    m_Logger.Debug("File matches pattern. File: '{0}'.", oldFileNameWithPath);
                                    matches.Add(Path.GetFileName(oldFileNameWithPath));
                                }
                            }

                            isValid = iterator.MoveNext();
                        }

                        m_Logger.Debug("{0} files matched for processing.", matches.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                m_Logger.Error("Exception encountered searching for files.", ex);
            }

            m_Logger.Debug("Completed looking for files to process.");

            return matches;
        }

        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="path">
        /// The path to look in.
        /// </param>
        /// <param name="fileName">
        /// The name of the file to delete.
        /// </param>
        /// <returns>
        /// True if the file was deleted.
        /// </returns>
        public bool DeleteFile(string path, string fileName, bool flush = true)
        {
            bool isDeleted = false;

            m_Logger.Debug("Attempting to delete file. Path: '{0}', Filename: '{1}'.", path, fileName);
            
            var filePath = BuildFilePath(path, fileName);
            try
            {
                var info = new FileInfo(filePath);
                if (info.Exists)
                {
                    m_Logger.Debug("File found for deletion. Path: '{0}', Filename: '{1}'.", path, fileName);
                    info.Delete();

                    if (flush) m_VolumeInfo.FlushAll();
                }
                else
                {
                    m_Logger.Debug("File not found for deletion. Path: '{0}', Filename: '{1}'.", path, fileName);                    
                }

                isDeleted = !info.Exists;

            }
            catch (Exception ex)
            {
                m_Logger.Error("Failed to delete file '{0}'.", ex, filePath);
                isDeleted = false;
            }

            return isDeleted; 
        }

        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="filePath">
        /// The fully qualified file path of the file to delete.
        /// </param>
        /// <returns>
        /// True if the file was deleted.
        /// </returns>
        public bool DeleteFile(string filePath, bool flush = true)
        {
            var path = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);

            return DeleteFile(path, fileName, flush);
        }

        /// <summary>
        /// Creates a file path.
        /// </summary>
        /// <param name="path">
        /// The directory/root path to alias.
        /// </param>
        /// <param name="fileName">
        /// The name of the file
        /// </param>
        /// <returns></returns>
        public string BuildFilePath(string path, string fileName)
        {
            return Path.Combine(path, fileName);
        }

        /// <summary>
        /// Indicates whether the directory exists.
        /// </summary>
        /// <param name="path">
        /// The directory path to check.
        /// </param>
        /// <returns>
        /// True if the directory exists.
        /// </returns>
        public bool DoesDirectoryExist(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// See <see cref="IFileHelper.Flush"/> for more details.
        /// </summary>
        public void Flush()
        {
            m_VolumeInfo.FlushAll();
        }
    }
}
