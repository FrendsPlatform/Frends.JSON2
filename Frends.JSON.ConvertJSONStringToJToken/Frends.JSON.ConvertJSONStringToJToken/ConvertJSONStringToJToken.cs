using Frends.JSON.ConvertJSONStringToJToken.Definitions;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Loader;

namespace Frends.JSON.ConvertJSONStringToJToken;

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
    /// Convert JSON string to JToken.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.JSON.ConvertJSONStringToJToken)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <returns>Object { bool Success, object Jtoken }</returns>
    public static Result ConvertJSONStringToJToken([PropertyTab] Input input)
    {
        return new Result(true, JToken.Parse(input.Json));
    }

    private static void OnPluginUnloadingRequested(AssemblyLoadContext obj)
    {
        obj.Unloading -= OnPluginUnloadingRequested;
    }
}