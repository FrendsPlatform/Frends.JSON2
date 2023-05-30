﻿using Frends.JSON.Validate.Definitions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Frends.JSON.Validate;

/// <summary>
/// JSON Task.
/// </summary>
public class JSON
{
    /// <summary>
    /// Validate your JSON with Json.NET Schema.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.JSON.Validate)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <param name="options">Optional parameter.</param>
    /// <returns>Object { bool Success, IEnumerable&lt;object&gt; Data }</returns>
    public static Result Validate([PropertyTab] Input input, [PropertyTab] Options options)
    {
        IList<string> errors;

        var schema = JSchema.Parse(input.JsonSchema);

        JToken jToken;

        try
        {
            jToken = GetJTokenFromInput(input.Json);
        }
        catch (System.Exception exception)
        {
            if (options.ThrowOnInvalidJson)
                throw;  // re-throw

            errors = new List<string>();
            while (exception != null)
            {
                errors.Add(exception.Message);
                exception = exception.InnerException;
            }

            return new Result(false, false, errors);
        }

        var isValid = jToken.IsValid(schema, out errors);

        if (!isValid && options.ThrowOnInvalidJson)
            throw new JsonException($"Json is not valid. {string.Join("; ", errors)}");

        return new Result(true, isValid, errors);
    }

    private static object GetJTokenFromInput(dynamic json)
    {
        if (json is string)
            return JToken.Parse(json);

        if (json is JToken)
            return json;

        throw new InvalidDataException("The input data was not recognized. Supported formats are JSON string and JToken.");
    }
}