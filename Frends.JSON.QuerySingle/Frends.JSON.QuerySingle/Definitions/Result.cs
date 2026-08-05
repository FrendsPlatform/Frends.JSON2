using System;

namespace Frends.JSON.QuerySingle.Definitions;

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
    /// Result data.
    /// </summary>
    /// <example>{{ "Name": "Foo", "Products": [{ "Name": "Bar", "Price": 1 }]}}</example>
    public dynamic Data { get; private set; }

    /// <summary>
    /// Error information when the task fails and ThrowErrorOnFailure is false.
    /// </summary>
    public Error Error { get; private set; }

    internal Result(bool success, object data)
    {
        Success = success;
        Data = data;
    }

    internal Result(bool success, Error error)
    {
        Success = success;
        Error = error;
    }
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
    /// Additional information about the error (exception).
    /// </summary>
    public Exception AdditionalInfo { get; set; }
}