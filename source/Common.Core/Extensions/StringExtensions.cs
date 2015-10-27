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
using System.Text;

namespace Ignite.Framework.Micro.Common.Core.Extensions
{
    /// <summary>
    /// General string extensions
    /// </summary>
    public static class StringExtensions
    {
       
        /// <summary>
        /// Replace all occurances of the 'find' string with the 'replace' string.
        /// </summary>
        /// <param name="content">Original string to operate on</param>
        /// <param name="find">String to find within the original string</param>
        /// <param name="replace">String to be used in place of the find string</param>
        /// <returns>Final string after all instances have been replaced.</returns>
        public static string Replace(this string content, string find, string replace)
        {
            int startFrom = 0;
            int findItemLength = find.Length;

            int firstFound = content.IndexOf(find, startFrom);
            StringBuilder returning = new StringBuilder();

            string workingString = content;

            while ((firstFound = workingString.IndexOf(find, startFrom)) >= 0)
            {
                returning.Append(workingString.Substring(0, firstFound));
                returning.Append(replace);

                // the remaining part of the string.
                workingString = workingString.Substring(firstFound + findItemLength, workingString.Length - (firstFound + findItemLength));
            }

            returning.Append(workingString);

            return returning.ToString();
        }

        /// <summary>
        /// Converts a string to a byte array
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this string content, Encoding encoding, int length)
        {
            byte[] bytes = new byte[length];

            var conversion = encoding.GetBytes(content);
            if (length < conversion.Length)
            {
                throw new ArgumentException("Length parameter is smaller than the byte array length required for content.");
            }

            Array.Copy(bytes, conversion, conversion.Length);

            return bytes;
        }

        /// <summary>
        /// Converts a string to a byte array
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this string content)
        {
            return ToByteArray(content, new UTF8Encoding(), content.Length);
        }

        /// <summary>
        /// Converts a string to a byte array
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this string content, int length)
        {
            return ToByteArray(content, new UTF8Encoding(), length);
        }

       
    }
}
