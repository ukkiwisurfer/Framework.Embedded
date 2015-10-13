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
    /// Extension methods for type check assertions.
    /// </summary>
    public static class TypeAssertions
    {
        /// <summary>
        /// Verifies that an object is of a given type.
        /// </summary>
        /// <param name="type">
        /// The type being verified.
        /// </param>
        /// <param name="item">
        /// The item being checked.
        /// </param>
        public static void ShouldBeOfType(this object item, Type type)
        {
            if (item.GetType() != type) throw new ArgumentException("Type of item does not match expected type."); 
        }

        /// <summary>
        /// Verifies that an object is of a given type.
        /// </summary>
        /// <param name="type">
        /// The type being verified.
        /// </param>
        /// <param name="item">
        /// The item being checked.
        /// </param>
        /// <param name="errorMessage">
        /// The error message to associate with the assertion, should it fail.
        /// </param>
        public static void ShouldBeOfType(this object item, Type type, string errorMessage)
        {
            if (item.GetType() != type) throw new ArgumentException(errorMessage); 
        }

        /// <summary>
        /// Verifies that an exception is of a specified type.
        /// </summary>
        /// <param name="type">
        /// The type to verify.
        /// </param>
        /// <param name="item">
        /// The exception being checked.
        /// </param>
        public static void OfType(this Exception item, Type type)
        {
            ShouldBeOfType(item, type);
        }
    }
}
