using System;

namespace Frends.JSON.Query.Definitions;

/// <summary>
/// Error information returned when the task fails and ThrowErrorOnFailure is false.
/// </summary>
public class Error
{
    /// <summary>
    /// Error message.
    /// </summary>
    public string Message { get; internal set; }

    /// <summary>
    /// The exception that caused the error.
    /// </summary>
    public Exception AdditionalInfo { get; internal set; }
}
