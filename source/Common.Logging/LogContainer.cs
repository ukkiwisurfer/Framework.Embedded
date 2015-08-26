using System;
using Microsoft.SPOT;

namespace Ignite.Infrastructure.Micro.Common.Logging
{
    using System.Collections;

    // <summary>
    /// Container for multiple <see cref="LogEntry"/> elements.
    /// </summary>
    public class LogContainer
    {
        /// <summary>
        /// Contains a collection of <see cref="LogEntry"/> items.
        /// </summary>
        public ArrayList LogEntries { get; set; }

        /// <summary>
        /// Initialises an instance of the <see cref="LogContainer"/> class.
        /// </summary>
        public LogContainer()
        {
            LogEntries = new ArrayList();
        }

        /// <summary>
        /// Initialises an instance of the <see cref="LogContainer"/> class.
        /// </summary>
        /// <param name="items">
        /// The items to populate the container with.
        /// </param>
        public LogContainer(object[] items) : this()
        {
            foreach (var item in items)
            {
                var entry = (LogEntry)item;
                if (entry != null)
                {
                    LogEntries.Add(entry);
                }
            }
        }
    }
}
