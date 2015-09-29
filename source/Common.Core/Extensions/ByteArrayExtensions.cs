
namespace Ignite.Framework.Micro.Common.Core.Extensions
{
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Converts an array of bytes to a hex string representation.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BytesToHexString(this byte[] bytes)
        {
            var builder = new StringBuilder();

            foreach (byte b in bytes)
            {
                builder.Append(StringUtility.Format("{0:X}", b));
            }

            return builder.ToString();
        }
    }
}
