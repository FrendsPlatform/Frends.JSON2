#pragma warning disable 1591

namespace Frends.JSON.EnforceTypes.Definitions
{
    /// <summary>
    /// Json data type enforcing rule
    /// </summary>
    public class JsonTypeRule
    {
        /// <summary>
        /// JSON path for the rule
        /// </summary>
        /// <example>$.prop1</example>
        public string JsonPath { get; set; }

        /// <summary>
        /// Data type to enforce
        /// </summary>
        /// <example>JsonDataType.Number</example>
        public JsonDataType DataType { get; set; }
    }
}
