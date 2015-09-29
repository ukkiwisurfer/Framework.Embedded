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
    using System.IO;

    /// <summary>
    /// Provides capabilities to manipulate and interact with a local file system.
    /// </summary>
    public interface IFileHelper
    {
        /// <summary>
        /// The current directory.
        /// </summary>
        /// <param name="path">
        /// The full directory path.
        /// </param>
        void SetCurrentDirectory(string path);

        /// <summary>
        /// Creates a directory.
        /// </summary>
        /// <param name="path">
        /// The full directory path.
        /// </param>
        void CreateDirectory(string path);

        /// <summary>
        /// Generates the name of the file.
        /// </summary>
        /// <returns>
        /// The name of the file
        /// </returns>
        string GenerateFileName(DateTime timestamp, string extension = ".tmp");

        /// <summary>
        /// Renames an existing file to a new file name.
        /// </summary>
        /// <param name="filePath">
        /// The path where the files can be found.
        /// </param>
        /// <param name="rawFileName">
        /// The name of the source file.
        /// </param>
        /// <param name="targetFileName">
        /// The name of the target file.
        /// </param>
        void RenameFile(string rawFileName, string targetFileName);

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
        void RenameAllFilesMatchingExtension(string sourcePath, string targetPath, string oldExtension, string newExtension);

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
        Stream OpenStream(string filePath, string fileName, int bufferSize = 1024);

        /// <summary>
        /// Returns all files matching the given search pattern for the specified folder.
        /// </summary>
        /// <param name="folder">
        /// The path to search in.
        /// </param>
        /// <param name="pattern">
        /// The file pattern to query for.
        /// </param>
        /// <returns>
        /// A collection of all filenames matching the search criteria.
        /// </returns>
        string[] GetAllFilesMatchingPattern(string folder, string pattern);

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
        bool DeleteFile(string path, string fileName);

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
        string BuildFilePath(string path, string fileName);

        /// <summary>
        /// Indicates whether the directory exists.
        /// </summary>
        /// <param name="path">
        /// The directory path to check.
        /// </param>
        /// <returns>
        /// True if the directory exists.
        /// </returns>
        bool DoesDirectoryExist(string path);
    }
}