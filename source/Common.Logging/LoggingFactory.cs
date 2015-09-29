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

namespace Ignite.Framework.Micro.Common.Logging
{
    using System;

    using Ignite.Framework.Micro.Common.Contract.Logging;

    public delegate ILogger CreateLogger(Type type);

    /// <summary>
    /// Defines a factory for creating logging providers.
    /// </summary>
    public class LoggingFactory : ILogFactory
    {
        private readonly CreateLogger m_Factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingFactory"/> class. 
        /// </summary>
        /// <param name="factory">
        /// Delegate callback to create a logging provider instance from. 
        /// </param>
        public LoggingFactory(CreateLogger factory)
        {
            m_Factory = factory;
        }

        /// <summary>
        /// See <see cref="ILogFactory.GetLogger"/> for more details.
        /// </summary>
        /// <param name="type">
        /// The data type to create the logging provider instance for.
        /// </param>
        /// <returns>
        /// The logging provider instance.
        /// </returns>
        public ILogger GetLogger(Type type)
        {
            return m_Factory(type);
        }
    }
}
