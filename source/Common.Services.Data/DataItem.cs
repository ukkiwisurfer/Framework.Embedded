namespace Ignite.Framework.Micro.Common.Services.Data
{
    using System;

    [Serializable]
    public class DataItem
    {
        /// <summary>
        /// The timestamp when the data item was captured.
        /// </summary>
        public DateTime CaptureTimestamp { get; set; }

        /// <summary>
        /// A name associated with the stream of data.
        /// </summary>
        public string DataStreamName { get; set; }

        /// <summary>
        /// The actual data payload.
        /// </summary>
        public string Payload { get; set; }
    }
}
