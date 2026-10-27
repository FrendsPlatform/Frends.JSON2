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
    public bool Success { get; set; }

    /// <summary>
    /// JToken.
    /// </summary>
    /// <example>{{ "?xml": { "@version": "1.0", "@standalone": "no" }, "root": { "foos": { "@id": "1", "foo": "bar" } }}}</example>
    public dynamic Jtoken { get; set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, Exception AdditionalInfo }</example>
    public Error Error { get; set; }
}