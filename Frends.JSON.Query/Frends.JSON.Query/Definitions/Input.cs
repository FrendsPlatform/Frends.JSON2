using System.ComponentModel.DataAnnotations;

namespace Frends.JSON.Query.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Json input needs to be of type string or JToken
    /// </summary>
    /// <example>{\"key\":\"value\"}</example>
    [DisplayFormat(DataFormatString = "Json")]
    public dynamic Json { get; set; }

    /// <summary>
    /// The query is of type JSONPath. 
    /// More details: http://goessner.net/articles/JsonPath/
    /// </summary>
    /// <example>\"$.key\"</example>
    public string Query { get; set; }
}