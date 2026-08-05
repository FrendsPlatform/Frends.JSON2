using Frends.JSON.QuerySingle.Definitions;
using Frends.JSON.QuerySingle.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;

namespace Frends.JSON.QuerySingle;

/// <summary>
/// JSON Task.
/// </summary>
public static class JSON
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
    /// Query JSON string/token for a single result.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.JSON.QuerySingle)
    /// </summary>
    /// <param name="input">Input parameters.</param>
    /// <param name="options">Optional parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Object { bool Success, dynamic Data, Error Error }</returns>
    public static Result QuerySingle([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            JToken jToken = GetJTokenFromInput(input.Json);
            JToken result = jToken.SelectToken(input.Query, options.ErrorWhenNotMatched);

            if (result == null && options.ErrorWhenNotMatched)
                throw new JsonException($"No matches found for query '{input.Query}'.");

            return new Result { Success = true, Data = result };
        }
        catch (Exception ex)
        {
            return ex.Handle(options);
        }
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