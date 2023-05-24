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
}