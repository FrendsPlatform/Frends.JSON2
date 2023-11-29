using Frends.JSON.ConvertXMLStringToJToken.Definitions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Xml;

namespace Frends.JSON.ConvertXMLStringToJToken;

/// <summary>
/// JSON Task.
/// </summary>
public class JSON
{
    /// <summary>
    /// Convert XML string to JToken.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.JSON.ConvertXMLStringToJToken)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <returns>Object { bool Success, object Jtoken }</returns>
    public static Result ConvertXMLStringToJToken([PropertyTab] Input input)
    {
        var doc = new XmlDocument();
        doc.LoadXml(input.XML);
        var jsonString = JsonConvert.SerializeXmlNode(doc);
        return new Result(true, JToken.Parse(jsonString));
    }
}