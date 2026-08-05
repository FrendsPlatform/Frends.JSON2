using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.JSON.QuerySingle.Definitions;

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
    /// Throw an exception if the task fails.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Custom error message to include when the task fails. Leave empty to use the default error message.
    /// </summary>
    /// <example></example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = string.Empty;
}