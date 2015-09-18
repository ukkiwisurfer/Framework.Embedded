
namespace Ignite.Framework.Micro.Common.Services.Data
{
    using System;
    using System.Collections;

    using Ignite.Framework.Micro.Common.Assertions;
    using Ignite.Framework.Micro.Common.Contract.Logging;
    using Ignite.Framework.Micro.Common.Logging;

    [Serializable]
    public class DataItemContainer
    {
        private readonly ArrayList m_DataItems;
        /// <summary>
        /// Contains a collection of <see cref="LogEntry"/> items.
        /// </summary>
        public ArrayList DataItems
        {
            get {  return m_DataItems; }
        }

        /// <summary>
        /// Initialises an instance of the <see cref="LogContainer"/> class.
        /// </summary>
        public DataItemContainer()
        {
            m_DataItems = new ArrayList();
        }

        /// <summary>
        /// Initialises an instance of the <see cref="LogContainer"/> class.
        /// </summary>
        /// <param name="items">
        /// The items to populate the container with.
        /// </param>
        public DataItemContainer(object[] items) : this()
        {
            items.ShouldNotBeNull();

            foreach (var item in items)
            {
                var entry = item as DataItem;
                if (entry != null)
                {
                    AddItem(entry);
                }
            }
        }

        /// <summary>
        /// Initialises an instance of the <see cref="LogContainer"/> class.
        /// </summary>
        /// <param name="items">
        /// The items to populate the container with.
        /// </param>
        public DataItemContainer(DataItem[] items) : this()
        {
            items.ShouldNotBeNull();

            foreach (var item in items)
            {
                AddItem(item);
            }
        }

        /// <summary>
        /// Adds a new <see cref="DataItem"/> to the container.
        /// </summary>
        /// <param name="entry">
        /// A single log entry to be added to the container.
        /// </param>
        public void AddItem(DataItem entry)
        {
            if (entry != null)
            {
                m_DataItems.Add(entry);
            }
        }

        /// <summary>
        /// Returns all the log entries managed by this container.
        /// </summary>
        /// <returns></returns>
        public DataItem[] GetEntries()
        {
            DataItem[] items = new DataItem[m_DataItems.Count];

            int index = 0;
            foreach (var logItem in m_DataItems)
            {
                items[index++] = (DataItem)logItem;
            }

            return items;
        }
    }
}