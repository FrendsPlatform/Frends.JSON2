using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.JSON.Validate.Definitions;

/// <summary>
/// Options parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// A flag to indicate whether invalid JSON should be treated as an error.
    /// When true, validation failure or parse error is treated as an error (subject to ThrowErrorOnFailure).
    /// When false, parse/validation errors are returned as a non-successful result with Success=false without going through error handling.
    /// </summary>
    /// <example>true</example>
    public bool FailOnInvalidJson { get; set; }

    /// <summary>
    /// If set to true, the task will throw an exception on failure.
    /// If set to false, the task returns a result with Success = false.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Optional custom error message used when ThrowErrorOnFailure is true or when returning a failed result.
    /// </summary>
    /// <example></example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = string.Empty;
}