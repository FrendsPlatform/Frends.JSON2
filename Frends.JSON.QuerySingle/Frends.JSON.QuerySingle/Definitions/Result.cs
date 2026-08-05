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
    public bool Success { get; set; }

    /// <summary>
    /// Result data.
    /// </summary>
    /// <example>{{ "Name": "Foo", "Products": [{ "Name": "Bar", "Price": 1 }]}}</example>
    public dynamic Data { get; set; }

    /// <summary>
    /// Error information when the task fails and ThrowErrorOnFailure is false.
    /// </summary>
    public Error Error { get; set; }
}
