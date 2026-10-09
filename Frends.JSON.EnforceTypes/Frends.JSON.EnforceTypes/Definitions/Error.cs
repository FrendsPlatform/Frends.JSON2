using System;

namespace Frends.JSON.EnforceTypes.Definitions
{
    /// <summary>
    /// Error details returned when the task fails and ThrowErrorOnFailure is false.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Error message describing what went wrong.
        /// </summary>
        /// <example>Failed to parse JSON input.</example>
        public string Message { get; set; }

        /// <summary>
        /// The exception that caused the error.
        /// </summary>
        /// <example>System.Exception: Value at '$.count' could not be converted to Integer.</example>
        public Exception AdditionalInfo { get; set; }
    }
}
