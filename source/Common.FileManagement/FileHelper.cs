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

namespace Ignite.Framework.Micro.Common.FileManagement
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Supports file operations.
    /// </summary>
    public class FileHelper : IFileHelper
    {
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
            builder.Append(".");
            builder.Append(extension);

            return builder.ToString();
        }

        /// <summary>
        /// Renames an existing file to a new file name.
        /// </summary>
        /// <param name="rawFileName">
        /// The name of the source file.
        /// </param>
        /// <param name="targetFileName">
        /// The name of the target file.
        /// </param>
        public void RenameFile(string rawFileName, string targetFileName)
        {
            if (File.Exists(targetFileName))
            {
                File.Delete(targetFileName);
            }

            File.Move(rawFileName, targetFileName);
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
        public void RenameAllFilesMatchingExtension(string sourcePath, string targetPath, string oldExtension, string newExtension)
        {
            var fileNames = Directory.EnumerateFiles(sourcePath);

            var patternCriteria = @"." + oldExtension;
            //var regular = new Regex(patternCriteria, RegexOptions.Compiled);

            foreach (string oldFileNameWithPath in fileNames)
            {
            //    if (regular.IsMatch(oldFileNameWithPath))
                if (oldFileNameWithPath.LastIndexOf(patternCriteria) > 0)
                {
                    string oldFileName = Path.GetFileName(oldFileNameWithPath);
                    string newFileName = Path.GetFileNameWithoutExtension(oldFileName) + "." + newExtension;
                    string newFileNameWithPath = this.BuildFilePath(targetPath, newFileName);

                    RenameFile(oldFileNameWithPath, newFileNameWithPath);
                }
            }
        }

        /// <summary>
        /// Opens/creates an existing/new file stream.
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
        public virtual Stream OpenStream(string filePath, string fileName, int bufferSize = 1024)
        {
            var fileNameWithPath = Path.Combine(filePath, fileName);
            return new FileStream(fileNameWithPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, bufferSize);
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

            var fileNames = Directory.EnumerateFiles(folder);
            var matches = new ArrayList();
            var iterator = fileNames.GetEnumerator();

            var isValid = iterator.MoveNext();
            if (isValid)
            {
                for (int fileIndex = 0; fileIndex < fileLimit; fileIndex++)
                {
                    if (!isValid) break;

                    var oldFileNameWithPath = iterator.Current as string;
                    if (oldFileNameWithPath != null)
                    {
                        var foundIndex = oldFileNameWithPath.LastIndexOf(patternCriteria);
                        if (foundIndex > 0)
                        {
                            matches.Add(Path.GetFileName(oldFileNameWithPath));
                        }
                    }

                    isValid = iterator.MoveNext();
                }
            }

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
        public bool DeleteFile(string path, string fileName)
        {
            var filePath = BuildFilePath(path, fileName);
            File.Delete(filePath);

            return !File.Exists(filePath);
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
    }
}
