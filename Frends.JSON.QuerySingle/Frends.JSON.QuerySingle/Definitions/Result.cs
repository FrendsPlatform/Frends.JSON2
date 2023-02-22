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
    public object Data { get; private set; }

    internal Result(bool success, object data)
    {
        Success = success;
        Data = data;
    }
}