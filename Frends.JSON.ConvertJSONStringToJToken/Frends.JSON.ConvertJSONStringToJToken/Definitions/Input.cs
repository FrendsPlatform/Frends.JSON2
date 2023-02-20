namespace Frends.JSON.ConvertJSONStringToJToken.Definitions;

/// <summary>
/// Input parameter.
/// </summary>
public class Input
{
    /// <summary>
    /// JSON string to convert.
    /// </summary>
    /// <example>{ 'foo': 'bar', 'foobar': ['Foo', 'Bar'] }</example>
    public string Json { get; set; }
}