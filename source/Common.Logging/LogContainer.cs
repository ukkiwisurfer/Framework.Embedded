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
        private readonly Queue m_LogEntries;
        private readonly object m_Synclock;
        private int m_Count;

        public int Count
        {
            get
            {
               return m_Count;
            }
        }

        /// <summary>
        /// Initialises an instance of the <see cref="LogContainer"/> class.
        /// </summary>
        public LogContainer()
        {
            m_LogEntries = new Queue();
            m_Synclock = new object();
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
                    m_Count = AddLogEntry(entry);
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
                m_Count = AddLogEntry(item);
            }
        }

        /// <summary>
        /// Adds a new <see cref="LogEntry"/> to the container.
        /// </summary>
        /// <param name="entry">
        /// A single log entry to be added to the container.
        /// </param>
        public int AddLogEntry(LogEntry entry)
        {
            int itemCount = 0;

            if (entry != null)
            {
                lock (m_Synclock)
                {
                    m_LogEntries.Enqueue(entry);

                    itemCount = m_LogEntries.Count;
                    m_Count = itemCount;
                }
            }

            return itemCount;
        }

        /// <summary>
        /// Returns all the log entries managed by this container.
        /// </summary>
        /// <returns>
        /// The next log entry (FIFO) if one exists in the container.
        /// </returns>
        public LogEntry GetNextEntry()
        {
            LogEntry logEntry = null;
            
            lock (m_Synclock)
            {
                if (m_LogEntries.Count > 0)
                {
                    logEntry =  m_LogEntries.Dequeue() as LogEntry;
                }
            }

            return logEntry;
        }
    }
}
