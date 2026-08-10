using System;

namespace Frends.JSON.Handlebars.Definitions;

/// <summary>
/// Error information returned when the task fails and ThrowErrorOnFailure is false.
/// </summary>
public class Error
{
    /// <summary>
    /// Human-readable error message.
    /// </summary>
    /// <example>Something went wrong.</example>
    public string Message { get; internal set; }

    /// <summary>
    /// The exception that caused the error, if available.
    /// </summary>
    public Exception AdditionalInfo { get; internal set; }
}
