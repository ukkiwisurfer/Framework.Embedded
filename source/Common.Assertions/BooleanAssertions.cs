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
    /// Extension methods for asserting validity for boolean data types.
    /// </summary>
    public static class BooleanAssertions
    {
        /// <summary>
        /// Asserts that the boolean evaluates as true.
        /// </summary>
        /// <param name="item">
        /// The boolean value been checked.
        /// </param>
        public static void ShouldBeTrue(this bool item)
        {
            if (item != true) throw new ArgumentException("Value is not true.");
        }

        /// <summary>
        /// Asserts that the boolean evaluates as true.
        /// </summary>
        /// <param name="item">
        /// The boolean value being checked.
        /// </param>
        /// <param name="errorMessage">
        /// The error message associated with failure to validate.
        /// </param>
        public static void ShouldBeTrue(this bool item, string errorMessage)
        {
            if (item != true) throw new ArgumentException(errorMessage);
        }

        /// <summary>
        /// Asserts that the boolean evaluates as false.
        /// </summary>
        /// <param name="item">
        /// The boolean value being checked.
        /// </param>
        public static void ShouldBeFalse(this bool item)
        {
            if (item != false) throw new ArgumentException("Value is not false.");
        }

        /// <summary>
        /// Asserts that the boolean evaluates as false.
        /// </summary>
        /// <param name="item">
        /// The boolean value being checked.
        /// </param>
        /// <param name="errorMessage">
        /// The error message associated with failure to validate.
        /// </param>
        public static void ShouldBeFalse(this bool item, string errorMessage)
        {
            if (item == true) throw new ArgumentException(errorMessage);
        }
    }
}
