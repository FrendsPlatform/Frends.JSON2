using Newtonsoft.Json.Linq;

#pragma warning disable 1591

namespace Frends.JSON.Transform.Definitions
{
    /// <summary>
    /// Return object with private setters
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Indicates whether the task completed successfully.
        /// </summary>
        /// <example>true</example>
        public bool Success { get; private set; }

        /// <summary>
        /// Transformation result as string
        /// </summary>
        /// <example>"{\"firstName\":\"veijo\"}"</example>
        public string Transformation { get; private set; }

        /// <summary>
        /// Transformation result as JToken
        /// </summary>
        /// <example>{"firstName": "veijo"}</example>
        public dynamic JToken { get; private set; }

        /// <summary>
        /// Error details when the task fails and ThrowErrorOnFailure is false.
        /// Null on success.
        /// </summary>
        /// <example>{ Message = "Json transformation failed: ...", AdditionalInfo = System.Exception }</example>
        public Error Error { get; private set; }

        public Result(string transformationResult, JToken jToken)
        {
            Success = true;
            Transformation = transformationResult;
            JToken = jToken;
        }

        internal Result(Error error)
        {
            Success = false;
            Error = error;
        }
    }
}
