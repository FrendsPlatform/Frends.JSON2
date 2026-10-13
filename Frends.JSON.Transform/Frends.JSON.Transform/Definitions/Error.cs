using System;

namespace Frends.JSON.Transform.Definitions
{
    /// <summary>
    /// Error details returned when a task failure occurs and ThrowErrorOnFailure is false.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Human-readable error message describing what went wrong.
        /// </summary>
        /// <example>Json transformation failed: ...</example>
        public string Message { get; private set; }

        /// <summary>
        /// The original exception that caused the failure.
        /// </summary>
        /// <example>System.FormatException: Input Json is not valid: ...</example>
        public Exception AdditionalInfo { get; private set; }

        internal Error(string message, Exception additionalInfo)
        {
            Message = message;
            AdditionalInfo = additionalInfo;
        }
    }
}
