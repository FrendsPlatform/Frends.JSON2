using System.ComponentModel;

namespace Frends.JSON.EnforceTypes.Definitions
{
    /// <summary>
    /// Options for the EnforceTypes task.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// If true, the task throws an exception on failure. If false, the task returns a result object with Success set to false and Error containing the error details.
        /// </summary>
        /// <example>true</example>
        [DefaultValue(true)]
        public bool ThrowErrorOnFailure { get; set; } = true;

        /// <summary>
        /// Custom error message to include when the task fails. If empty, the original exception message is used.
        /// </summary>
        /// <example>An error occurred while enforcing types in the JSON document.</example>
        [DefaultValue("")]
        public string ErrorMessageOnFailure { get; set; } = string.Empty;
    }
}
