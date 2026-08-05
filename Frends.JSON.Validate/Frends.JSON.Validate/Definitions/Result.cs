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
    public string Message { get; set; }

    /// <summary>
    /// Additional error information.
    /// </summary>
    public Exception AdditionalInfo { get; set; }
}