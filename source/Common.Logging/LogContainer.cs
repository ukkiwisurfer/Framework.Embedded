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
    using System.Collections;

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Logging;

    /// <summary>
    /// Container for multiple <see cref="LogEntry"/> elements.
    /// </summary>
    public class LogContainer
    {
        /// <summary>
        /// Contains a collection of <see cref="LogEntry"/> items.
        /// </summary>
        private readonly ArrayList m_LogEntries;

        /// <summary>
        /// Initialises an instance of the <see cref="LogContainer"/> class.
        /// </summary>
        public LogContainer()
        {
            m_LogEntries = new ArrayList();
        }

        /// <summary>
        /// Initialises an instance of the <see cref="LogContainer"/> class.
        /// </summary>
        /// <param name="items">
        /// The items to populate the container with.
        /// </param>
        public LogContainer(object[] items) : this()
        {
            items.ShouldNotBeNull();

            foreach (var item in items)
            {
                var entry = item as LogEntry;
                if (entry != null)
                {
                    AddLogEntry(entry);
                }
            }
        }

        /// <summary>
        /// Initialises an instance of the <see cref="LogContainer"/> class.
        /// </summary>
        /// <param name="items">
        /// The items to populate the container with.
        /// </param>
        public LogContainer(LogEntry[] items) : this()
        {
            items.ShouldNotBeNull();

            foreach (var item in items)
            {
                AddLogEntry(item);
            }
        }

        /// <summary>
        /// Adds a new <see cref="LogEntry"/> to the container.
        /// </summary>
        /// <param name="entry">
        /// A single log entry to be added to the container.
        /// </param>
        public void AddLogEntry(LogEntry entry)
        {
            if (entry != null)
            {
                m_LogEntries.Add(entry);
            }
        }

        /// <summary>
        /// Returns all the log entries managed by this container.
        /// </summary>
        /// <returns></returns>
        public LogEntry[] GetLogEntries()
        {
            LogEntry[] items = new LogEntry[m_LogEntries.Count];

            int index = 0;
            foreach (var logItem in m_LogEntries)
            {
                items[index++] = (LogEntry) logItem;
            }

            return items;
        }
    }
}
