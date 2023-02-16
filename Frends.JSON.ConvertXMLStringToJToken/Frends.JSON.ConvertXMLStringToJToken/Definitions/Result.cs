namespace Frends.JSON.ConvertXMLStringToJToken.Definitions;

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
    /// <example>{{ "?xml": { "@version": "1.0", "@standalone": "no" }, "root": { "foos": { "@id": "1", "foo": "bar" } }}}</example>
    public object Jtoken { get; private set; }

    internal Result(bool success, object jtoken)
    {
        Success = success;
        Jtoken = jtoken;
    }
}