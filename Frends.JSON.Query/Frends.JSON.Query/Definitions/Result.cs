using System.Collections.Generic;

namespace Frends.JSON.Query.Definitions;

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    /// <summary>
    /// Operation complete without errors.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; internal set; }

    /// <summary>
    /// Result data.
    /// </summary>
    /// <example>[ { Foo }, { Bar } ]</example>
    public IEnumerable<object> Data { get; internal set; }

    /// <summary>
    /// Error information, populated when the task fails and ThrowErrorOnFailure is false.
    /// </summary>
    public Error Error { get; internal set; }
}