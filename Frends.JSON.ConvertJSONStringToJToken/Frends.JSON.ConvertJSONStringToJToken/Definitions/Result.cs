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
    public bool Success { get; private set; }

    /// <summary>
    /// JToken.
    /// </summary>
    /// <example>{{ "foo": "bar", "foobar": [ "Foo", "Bar" ]}}</example>
    public object Jtoken { get; private set; }

    internal Result(bool success, object jtoken)
    {
        Success = success;
        Jtoken = jtoken;
    }
}