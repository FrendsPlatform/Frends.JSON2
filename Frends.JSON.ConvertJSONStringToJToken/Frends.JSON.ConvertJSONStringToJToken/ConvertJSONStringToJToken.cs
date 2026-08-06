using Frends.JSON.ConvertJSONStringToJToken.Definitions;
using Frends.JSON.ConvertJSONStringToJToken.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Threading;

namespace Frends.JSON.ConvertJSONStringToJToken;

/// <summary>
/// JSON Task.
/// </summary>
public static class JSON
{
    /// <summary>
    /// Convert JSON string to JToken.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.JSON.ConvertJSONStringToJToken)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <param name="options">Options for error handling</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>Object { bool Success, dynamic Jtoken, Error Error }</returns>
    public static Result ConvertJSONStringToJToken([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            return new Result { Success = true, Jtoken = JToken.Parse(input.Json) };
        }
        catch (Exception ex)
        {
            return ex.Handle(options);
        }
    }
}
