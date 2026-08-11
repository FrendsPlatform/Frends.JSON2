using Frends.JSON.Validate.Definitions;
using Frends.JSON.Validate.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace Frends.JSON.Validate;

/// <summary>
/// JSON Task.
/// </summary>
public static class JSON
{
    /// <summary>
    /// Validate your JSON with Json.NET Schema.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.JSON.Validate)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <param name="options">Optional parameter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Object { bool Success, bool IsValid, IList&lt;string&gt; Errors, Error Error }</returns>
    public static Result Validate([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        try
        {
            SchemaActivation.Activate();
            JSchema schema;
            IList<string> errors;
            JToken jToken;

            try
            {
                schema = JSchema.Parse(input.JsonSchema);
                jToken = GetJTokenFromInput(input.Json);
            }
            catch (Exception exception)
            {
                if (options.FailOnInvalidJson)
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

            if (!isValid && options.FailOnInvalidJson)
                throw new JsonException($"Json is not valid. {string.Join("; ", errors)}");

            return new Result(true, isValid, errors);
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
}