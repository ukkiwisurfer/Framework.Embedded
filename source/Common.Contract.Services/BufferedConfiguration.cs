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

namespace Ignite.Framework.Micro.Common.Contract.Services
{
    /// <summary>
    /// Configuration information for received data that is being buffered.
    /// </summary>
    public class BufferedConfiguration
    {
        /// <summary>
        /// The path where working files are stored.
        /// </summary>
        public string WorkingPath { get; set; }

        /// <summary>
        /// The file extension that working files are suffixed with.
        /// </summary>
        public string WorkingFileExtension { get; set; }

        /// <summary>
        /// The path where completed data files are stored.
        /// </summary>
        public string TargetPath { get; set; }

        /// <summary>
        /// The file extension that completed data files are suffuxed with.
        /// </summary>
        public string TargetFileExtension { get; set; }
    }
}
