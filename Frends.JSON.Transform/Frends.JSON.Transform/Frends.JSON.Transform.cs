using Newtonsoft.Json.Linq;
using Frends.JSON.Transform.Definitions;
using Frends.JSON.Transform.Helpers;
using System.ComponentModel;
using System.Threading;
using JUST;

namespace Frends.JSON.Transform
{
    /// <summary>
    /// Main class of the Task
    /// </summary>
    public static class JSON
    {
        /// <summary>
        /// Maps input json using JUST.Net library. 
        /// [Documentation](https://tasks.frends.com/tasks#frends-tasks/Frends.JSON.Transform)
        /// JUST.Net documentation: 'https://github.com/WorkMaze/JUST.net#just'
        /// </summary>
        /// <param name="input">Input parameters</param>
        /// <param name="options">Task options</param>
        /// <param name="cancellationToken">Token to cancel the task.</param>
        /// <returns>Object { bool Success, string Transformation, JToken JToken, Error Error }</returns>
        public static Result Transform([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                string result;
                //Try parse input Json for simple validation
                try
                {
                    JToken.Parse(input.InputJson.ToString());
                }
                catch (Exception ex)
                {
                    throw new FormatException("Input Json is not valid: " + ex.Message);
                }
                try
                {
                    // Throw if JsonInput's root element is JArray
                    if (JToken.Parse(input.InputJson.ToString()) is JArray)
                        throw new FormatException("Input Json is not valid: Array is not supported as root element.");
                    var transformer = new JsonTransformer();
                    result = transformer.Transform(input.JsonMap, input.InputJson.ToString());

                }
                catch (Exception ex)
                {
                    throw new Exception("Json transformation failed: " + ex.Message, ex);
                }

                return new Result(result, JToken.Parse(result));
            }
            catch (Exception ex)
            {
                return ex.Handle(options);
            }
        }
    }
}
