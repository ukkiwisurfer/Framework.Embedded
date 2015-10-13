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

namespace Ignite.Framework.Micro.Common.Errors
{
    using System;
    using Ignite.Framework.Micro.Common.Core;

    /// <summary>
    /// Indicates the Error severity.
    /// </summary>
    public enum Severity
    {
        /// <summary>
        /// No error.
        /// </summary>
        NoError = 0,

        /// <summary>
        /// Information only.
        /// </summary>
        Information = 10,

        /// <summary>
        /// Warnings raised.
        /// </summary>
        Warning = 20,

        /// <summary>
        /// Error detected.
        /// </summary>
        Error = 30,

        /// <summary>
        /// Critical system failure.
        /// </summary>
        Critical = 40
    }

    /// <summary>
    /// Category indicators for Errors. 
    /// <list type="table">
    /// <listheader>
    /// <term>Category</term>
    /// <description>Interpretation</description></listheader>
    /// <item>
    /// <term>None</term>
    /// <description>No error category assigned</description>
    /// </item>
    /// <item>
    /// <term>Unhandled Error</term>
    /// <description>In good old COM style ‘Error – an unspecified error has occurred’</description>
    /// </item>
    /// <item>
    /// <term>Infrastructure Error</term>
    /// <description>An Error that happens before we get into the application logic</description>
    /// </item>
    /// <item>
    /// <term>Application Error</term>
    /// <description>An Error that happens within the application logic</description>
    /// </item>
    /// </list>
    /// </summary>
    public enum ErrorCategory
    {
        /// <summary>
        /// No error category assigned.
        /// </summary>
        None = 0,

        /// <summary>
        /// In good old COM style ‘Error – an unspecified error has occurred’.
        /// </summary>
        Unhandled = 1,

        /// <summary>
        /// An error that happens before we get into the application logic.
        /// </summary>
        Infrastructure = 2,

        /// <summary>
        /// An Error that happens within the application logic.
        /// </summary>
        Application = 3
    }

    /// <summary>
    /// Type indicator for errors.
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>Type</term>
    /// <description>Interpretation</description>
    /// </listheader>
    /// <item>
    /// <term>None</term>
    /// <description>No error type assigned</description>
    /// </item>
    /// <item>
    /// <term>General Error</term>
    /// <description>Unhandled Application Exceptions</description>
    /// </item>
    /// <item>
    /// <term>Validation Error</term>
    /// <description>Errors in Validating the input</description>
    /// </item>
    /// <item>
    /// <term>Operation Error</term>
    /// <description>Any other error in the application</description>
    /// </item>
    /// <item>
    /// <term>Integration Error</term>
    /// <description>Integration layer exception</description>
    /// </item>
    /// </list>
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// No error detected.
        /// </summary>
        None = 0,

        /// <summary>
        /// General error detected.
        /// </summary>
        General = 1,

        /// <summary>
        /// Validation error detected.
        /// </summary>
        Validation = 2,

        /// <summary>
        /// Error occured when calling an operation.
        /// </summary>
        Operation = 3,

        /// <summary>
        /// Error occured when either passing across to or executing in integration layer.
        /// </summary>
        Integration = 4
    }

    /// <summary>
    /// Represents an ErrorCode, constructed by combining Severity, Category and Type of error.
    /// </summary>
    public class ErrorCode
    {
        private readonly Severity m_Severity;
        private readonly ErrorCategory m_ErrorCategory;
        private readonly ErrorType m_ErrorType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCode"/> class. 
        /// </summary>
        /// <param name="severity">
        /// The Severity of the error.
        /// </param>
        /// <param name="category">
        /// The Category the error belongs to.
        /// </param>
        /// <param name="type">
        /// The type of the error.
        /// </param>
        public ErrorCode(Severity severity, ErrorCategory category, ErrorType type)
        {
            this.m_Severity = severity;
            this.m_ErrorCategory = category;
            this.m_ErrorType = type;
        }

        /// <summary>
        /// Validate that the provided integer is within range for the given Enumerated Type.
        /// </summary>
        /// <param name="value">The integer to range test</param>
        /// <param name="enumType">The type of the enumeration.</param>
        /// <returns>The provided value, if the test passes, otherwise throws an InvalidCastException.</returns>
        private static int ValidateEnum(int value, Type enumType)
        {
            //if (Enum.IsDefined(enumType, value))
            //{
            //    return value;
            //}

            //throw new FormatException(StringUtility.Format("Cannot cast {0} into the {1} type, please check the documentation for valid ErrorCode ranges.", value, enumType.Name));
            return value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCode"/> class. 
        /// </summary>
        /// <remarks>
        /// The fragility of the casting in this case is deliberate as we do not want to instantiate a class against an invalid code.
        /// </remarks>
        /// <param name="code">
        /// A literal code. Must be a nine digit numeric string.
        /// </param>
        public ErrorCode(string code)
        {
            if (Code.Length == 11)
            {
                int severityCheck = ValidateEnum(Convert.ToInt32(code.Substring(0, 2)), typeof(Severity));
                this.m_Severity = (Severity)severityCheck;

                int categoryCheck = ValidateEnum(Convert.ToInt32(code.Substring(3, 3)), typeof(ErrorCategory));
                this.m_ErrorCategory = (ErrorCategory)categoryCheck;

                int errorTypeCheck = ValidateEnum(Convert.ToInt32(code.Substring(8)), typeof(ErrorType));
                this.m_ErrorType = (ErrorType)errorTypeCheck;
            }
            else
            {
                throw new FormatException(StringUtility.Format("Cannot cast {0} as a valid ErrorCode - must be a nine-digit numeric string.", code));
            }

        }

        /// <summary>
        /// The Severity of the error.
        /// </summary>
        public Severity Severity
        {
            get { return this.m_Severity; }
        }

        /// <summary>
        /// The Category of the error.
        /// </summary>
        public ErrorCategory Category
        {
            get { return this.m_ErrorCategory; }
        }

        /// <summary>
        /// The ErrorType of the error.
        /// </summary>
        public ErrorType ErrorType
        {
            get { return this.m_ErrorType; }
        }

        /// <summary>
        /// The ErrorCode, constructed as a nine digit string.
        /// </summary>
        public string Code
        {
            get
            {
                // Double conversion because Enum doesn't support indexed format strings.
                return StringUtility.Format("{0:D2},{1:D3},{2:D4}", this.m_Severity.ToString(), this.m_ErrorCategory.ToString(), this.m_ErrorType.ToString());
            }
        }
    }

}
