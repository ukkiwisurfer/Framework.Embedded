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

namespace Ignite.Framework.Micro.Common.Contract.Services
{
    /// <summary>
    /// Defines the operations and properties associated with a multi-threaded service.
    /// </summary>
    public interface IThreadedService : IService
    {
        /// <summary>
        /// Indicates whether processing of incoming requests is enabled.
        /// </summary>
        bool IsProcessingEnabled { get; set; }

        /// <summary>
        /// Provides the ability to turn off and on logging, when a logging provider
        /// has been provided at construction time.
        /// </summary>
        bool IsLoggingEnabled { get; set; }

        /// <summary>
        /// The duration to sleep for before rechecking the cancellation token.
        /// </summary>
        int SleepPeriodInMilliseconds { get; set; }

        /// <summary>
        /// The duration to wait for before forcing shutdown.
        /// </summary>
        int WaitForShutdownPeriodInMilliseconds { get; set; }
       
    }
}