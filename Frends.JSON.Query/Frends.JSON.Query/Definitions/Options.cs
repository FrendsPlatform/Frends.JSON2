using System.ComponentModel;

namespace Frends.JSON.Query.Definitions;

/// <summary>
/// Options parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// A flag to indicate whether an error should be thrown if no tokens are found when evaluating part of the expression.
    /// </summary>
    /// <example>true</example>
    public bool ErrorWhenNotMatched { get; set; }

    /// <summary>
    /// If true, exceptions are thrown on failure. If false, errors are returned as part of the result.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Custom error message to use when an error occurs. Leave empty to use the original exception message.
    /// </summary>
    /// <example></example>
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = string.Empty;
}