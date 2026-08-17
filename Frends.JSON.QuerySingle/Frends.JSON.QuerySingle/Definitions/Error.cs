using System;

namespace Frends.JSON.QuerySingle.Definitions;

/// <summary>
/// Error details.
/// </summary>
public class Error
{
    /// <summary>
    /// Error message.
    /// </summary>
    /// <exampleQuery failed unexpectedly.</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional information about the error (exception).
    /// </summary>
    /// <example>object { Exception AdditionalInfo }</example>
    public Exception AdditionalInfo { get; set; }
}
