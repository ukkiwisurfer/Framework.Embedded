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

using System;

namespace Ignite.Framework.Micro.Common.Core.Extensions
{
    /// <summary>
    /// Extension methods for converting arrays of bytes.
    /// </summary>
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

        /// <summary>
        /// Copies 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void CopyTo(this byte[] source, byte[] destination)
        {
            if (source.Length > destination.Length)
            {
                throw new ArgumentException("Source array is larger than the destination byte array required for copying.");
            }

            Array.Copy(source,destination,source.Length);
        }
    }
}
