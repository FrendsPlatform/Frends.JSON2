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
    public bool Success { get; private set; }

    /// <summary>
    /// Result data.
    /// </summary>
    /// <example>[ { Foo }, { Bar } ]</example>
    public IEnumerable<object> Data { get; private set; }

    internal Result(bool success, IEnumerable<object> data)
    {
        Success = success;
        Data = data;
    }
}