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

namespace Ignite.Framework.Micro.Common.Core
{
    /// <summary>
    /// Provides additional parsing operations
    /// </summary>
    public abstract class Parse
    {
        /// <summary>
        /// Attempt to parse the provided string value.
        /// </summary>
        /// <param name="s">String value to be parsed</param>
        /// <param name="i">Variable to set successfully parsed value to</param>
        /// <returns>True if parsing was successful</returns>
        public static bool TryParseInt(string s, out int i)
        {
            i = 0;
            try
            {
                i = int.Parse(s);
                return true;
            }
            catch 
            {
                return false;
            }    
        }

        /// <summary>
        /// Attempt to parse the provided string value.
        /// </summary>
        /// <param name="s">String value to be parsed</param>
        /// <param name="i">Variable to set successfully parsed value to</param>
        /// <returns>True if parsing was successful</returns>
        public static bool TryParseShort(string s, out short i)
        {
            i = 0;
            try
            {
                i = short.Parse(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempt to parse the provided string value.
        /// </summary>
        /// <param name="s">String value to be parsed</param>
        /// <param name="i">Variable to set successfully parsed value to</param>
        /// <returns>True if parsing was successful</returns>
        public static bool TryParseLong(string s, out long i)
        {
            i = 0;
            try
            {
                i = long.Parse(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempt to parse the provided string value.
        /// </summary>
        /// <param name="s">String value to be parsed</param>
        /// <param name="i">Variable to set successfully parsed value to</param>
        /// <returns>True if parsing was successful</returns>
        public static bool TryParseDouble(string s, out double i)
        {
            i = 0;
            try
            {
                i = double.Parse(s);
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Attempt to parse the provided string value.
        /// </summary>
        /// <param name="s">String value to be parsed</param>
        /// <param name="val">Variable to set successfully parsed value to</param>
        /// <returns>True if parsing was successful</returns>
        public static bool TryParseBool(string s, out bool val)
        {
            val = false;
            try
            {
                if (s == "1" || s.ToUpper() == bool.TrueString.ToUpper())
                {
                    val = true;

                    return true;
                }
                else if (s == "0" || s.ToUpper() == bool.FalseString.ToUpper())
                {
                    val = false;

                    return true;
                }

                return false;

            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempt to parse the provided string value.
        /// </summary>
        /// <param name="s">String value to be parsed</param>
        /// <param name="i">Variable to set successfully parsed value to</param>
        /// <returns>True if parsing was successful</returns>
        public static bool TryParseUInt(string s, out uint i)
        {
            i = 0;
            try
            {
                i = uint.Parse(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempt to parse the provided string value.
        /// </summary>
        /// <param name="s">String value to be parsed</param>
        /// <param name="i">Variable to set successfully parsed value to</param>
        /// <returns>True if parsing was successful</returns>
        public static bool TryParseUShort(string s, out ushort i)
        {
            i = 0;
            try
            {
                i = ushort.Parse(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempt to parse the provided string value.
        /// </summary>
        /// <param name="s">String value to be parsed</param>
        /// <param name="i">Variable to set successfully parsed value to</param>
        /// <returns>True if parsing was successful</returns>
        public static bool TryParseULong(string s, out ulong i)
        {
            i = 0;
            try
            {
                i = ulong.Parse(s);
                return true;
            }
            catch
            {
                return false;
            }
        }



    }

}
