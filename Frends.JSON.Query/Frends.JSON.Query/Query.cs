using Frends.JSON.Query.Definitions;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Frends.JSON.Query;

/// <summary>
/// JSON Task.
/// </summary>
public class JSON
{
    /// Mem cleanup.
    static JSON()
    {
        var currentAssembly = Assembly.GetExecutingAssembly();
        var currentContext = AssemblyLoadContext.GetLoadContext(currentAssembly);
        if (currentContext != null)
            currentContext.Unloading += OnPluginUnloadingRequested;
    }

    /// <summary>
    /// Query JSON string/token.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.JSON.Query)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <param name="options">Optional parameter.</param>
    /// <returns>Object { bool Success, IEnumerable&lt;object&gt; Data }</returns>
    public static Result Query([PropertyTab] Input input, [PropertyTab] Options options)
    {
        JToken jToken = GetJTokenFromInput(input.Json);
        return new Result(true, jToken.SelectTokens(input.Query, options.ErrorWhenNotMatched));
    }

    private static object GetJTokenFromInput(dynamic json)
    {
        if (json is string)
            return JToken.Parse(json);

        if (json is JToken)
            return json;

        throw new InvalidDataException("The input data was not recognized. Supported formats are JSON string and JToken.");
    }

    private static void OnPluginUnloadingRequested(AssemblyLoadContext obj)
    {
        obj.Unloading -= OnPluginUnloadingRequested;
    }
}