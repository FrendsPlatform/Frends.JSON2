using System.ComponentModel.DataAnnotations;

namespace Frends.JSON.Validate.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Json input needs to be of type string or JToken.
    /// </summary>
    /// <example>{\"key\":\"value\"}</example>
    [DisplayFormat(DataFormatString = "Json")]
    public dynamic Json { get; set; }

    /// <summary>
    /// Json Schema to validate to. Uses Newtonsoft JsonSchema
    /// </summary>
    /// <example>{\"type\": \"object\", \"properties\": {\"name\": {\"type\":\"string\"} } }</example>
    [DisplayFormat(DataFormatString = "Json")]
    public string JsonSchema { get; set; }
}