using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Frends.JSON.QuerySingle.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// JSON input needs to be of type string or JToken
    /// </summary>
    /// <example>{\"key\":\"value\"}</example>
    [DisplayFormat(DataFormatString = "Json")]
    public dynamic Json { get; set; }

    /// <summary>
    /// The query is of type JSONPath. 
    /// More details: http://goessner.net/articles/JsonPath/
    /// </summary>
    [DefaultValue("\"$.key\"")]
    public string Query { get; set; }
}