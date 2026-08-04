using System;

namespace Frends.JSON.ConvertJSONStringToJToken.Definitions;

/// <summary>
/// Error information.
/// </summary>
public class Error
{
    /// <summary>
    /// Error message.
    /// </summary>
    /// <example>An error occurred.</example>
    public string Message { get; init; }

    /// <summary>
    /// Additional error information.
    /// </summary>
    public Exception AdditionalInfo { get; init; }
}
