using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.JSON.ConvertJSONStringToJToken.Definitions;

/// <summary>
/// Options for the task.
/// </summary>
public class Options
{
    /// <summary>
    /// Whether to throw an error on failure or return a result with Success = false.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Custom error message to use when ThrowErrorOnFailure is true.
    /// </summary>
    /// <example></example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = string.Empty;
}
