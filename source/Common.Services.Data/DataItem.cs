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
        /// The actual data payload.
        /// </summary>
        public byte[] Payload { get; set; }
    }
}
