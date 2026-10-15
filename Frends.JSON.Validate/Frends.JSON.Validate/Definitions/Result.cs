using System;
using System.Collections.Generic;

namespace Frends.JSON.Validate.Definitions;

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    /// <summary>
    /// Operation complete without errors.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// JSON was valid.
    /// </summary>
    /// <example>true</example>
    public bool IsValid { get; set; }

    /// <summary>
    /// List of errors.
    /// </summary>
    /// <example>{ An error occured..., Another error }</example>
    public IList<string> Errors { get; set; }

    /// <summary>
    /// Error information when Success is false.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; set; }

    internal Result(bool success, bool isValid, IList<string> errors)
    {
        Success = success;
        IsValid = isValid;
        Errors = errors;
    }

    internal Result() { }
}

/// <summary>
/// Error details.
/// </summary>
public class Error
{
    /// <summary>
    /// Error message.
    /// </summary>
    /// <example>Invalid JSON: unexpected character encountered</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional error information.
    /// </summary>
    /// <example>System.Exception: Invalid JSON</example>
    public Exception AdditionalInfo { get; set; }
}