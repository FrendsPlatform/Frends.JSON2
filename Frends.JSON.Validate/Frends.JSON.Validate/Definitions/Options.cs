using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.JSON.Validate.Definitions;

/// <summary>
/// Options parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// A flag to indicate whether an error should be thrown if JSON was invalid.
    /// </summary>
    /// <example>true</example>
    public bool ThrowOnInvalidJson { get; set; }

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