using Frends.JSON.ConvertJSONStringToJToken.Definitions;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace Frends.JSON.ConvertJSONStringToJToken;

/// <summary>
/// JSON Task.
/// </summary>
public class JSON
{
    /// <summary>
    /// Convert JSON string to JToken.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.JSON.ConvertJSONStringToJToken)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <returns>Object { bool Success, dynamic Jtoken }</returns>
    public static Result ConvertJSONStringToJToken([PropertyTab] Input input)
    {
        return new Result(true, JToken.Parse(input.Json));
    }
}