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
    /// Extension methods for asserting validity checks against class instances.
    /// </summary>
    public static class GenericAssertions
    {
        /// <summary>
        /// Verifies that an object is not null.
        /// </summary>
        /// <param name="instance"></param>
        public static void ShouldNotBeNull(this object instance)
        {
            if (instance == null) throw new ArgumentNullException();
        }
    }
}
