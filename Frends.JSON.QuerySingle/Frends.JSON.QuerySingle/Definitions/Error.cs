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
    public string Message { get; set; }

    /// <summary>
    /// Additional information about the error (exception).
    /// </summary>
    public Exception AdditionalInfo { get; set; }
}
