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

namespace Ignite.Framework.Micro.Common.Assertions
{
    using System;

    /// <summary>
    /// Defines extension methods for string types that assert if conditions are not met. 
    /// </summary>
    public static class StringAssertions
    {
        /// <summary>
        /// Asserts if the string is null or empty.
        /// </summary>
        /// <param name="text">
        /// The text to be verified.
        /// </param>
        public static void ShouldNotBeEmpty(this string text)
        {
            if ((text == null) || (string.Empty.Equals(text))) throw new ArgumentNullException();
        }
    }
}
