using System.ComponentModel;

namespace Frends.JSON.Transform.Definitions
{
    /// <summary>
    /// Options for the Transform task.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// If true, an exception is thrown on failure.
        /// If false, the error details are returned in the Result instead.
        /// </summary>
        /// <example>true</example>
        [DefaultValue(true)]
        public bool ThrowErrorOnFailure { get; set; } = true;

        /// <summary>
        /// Custom error message to include when a failure occurs.
        /// Leave empty to use the original exception message.
        /// </summary>
        /// <example>Transform failed due to invalid input</example>
        [DefaultValue("")]
        public string ErrorMessageOnFailure { get; set; }
    }
}
