

namespace Ignite.Framework.Micro.Common.Logging
{
    using System;
    using Ignite.Framework.Micro.Common.Core;
    using Microsoft.SPOT;

    public class ConsoleLogger
    {
        public void Log(string message, params object[] parameters)
        {
            var timestamp = DateTime.UtcNow;

            var builder = new StringBuilder(timestamp.ToString("yyyy-MM-dd HH:mm:ss."));
            builder.Append(timestamp.Millisecond.ToString("D3"));
            builder.Append(" | ");
            builder.Append(StringUtility.Format(message, parameters));

            Debug.Print(builder.ToString());
        }

        public void LogFreeMemory()
        {
            var free = Debug.GC(true);
            Log("Memory free: {0} bytes.", free);
        }
    }
}
