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
    /// Defines the operations and properties applicable for defining configuration
    /// details for buffering data.
    /// </summary>
    public interface IBufferConfiguration
    {
        /// <summary>
        /// The size of the buffer to create (in bytes).
        /// </summary>
        int BufferSizeInBytes { get; set; }

    }
}