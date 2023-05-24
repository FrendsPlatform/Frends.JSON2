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
    public bool Success { get; private set; }

    /// <summary>
    /// JSON was valid.
    /// </summary>
    /// <example>true</example>
    public bool IsValid { get; set; }

    /// <summary>
    /// List of errors.
    /// </summary>
    /// <example>{ An error occured... }</example>
    public IList<string> Errors { get; set; }

    internal Result(bool success, bool isValid, IList<string> errors)
    {
        Success = success;
        IsValid = isValid;
        Errors = errors;
    }
}