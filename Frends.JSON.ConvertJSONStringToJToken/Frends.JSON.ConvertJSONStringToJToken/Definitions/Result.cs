namespace Frends.JSON.ConvertJSONStringToJToken.Definitions;

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    /// <summary>
    /// Operation complete without errors.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; init; }

    /// <summary>
    /// JToken.
    /// </summary>
    /// <example>{{ "foo": "bar", "foobar": [ "Foo", "Bar" ]}}</example>
    public dynamic Jtoken { get; init; }

    /// <summary>
    /// Error information if the operation failed.
    /// </summary>
    public Error Error { get; init; }
}