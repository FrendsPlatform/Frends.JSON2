namespace Frends.JSON.EnforceTypes.Definitions
{
    /// <summary>
    /// Return object for the EnforceTypes task.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Indicates whether the task completed successfully.
        /// </summary>
        /// <example>true</example>
        public bool Success { get; private set; }

        /// <summary>
        /// Error details. Null when the task succeeds.
        /// </summary>
        /// <example>{"Message": "Value at '$.count' could not be converted to Integer.", "AdditionalInfo": {}}</example>
        public Error Error { get; private set; }

        /// <summary>
        /// The processed JSON as a string. Null when the task fails.
        /// </summary>
        /// <example>"{\"hello\": 123,\"hello_2\": 123.5,\"world\": true,\"bad_arr\": [\"hello, world\"],\"bad_arr_2\": [{\"prop1\": 123}],\"good_arr\": [\"hello, world\"],\"good_arr_2\": [{\"prop1\": 123}]}"</example>
        public string JsonAsString { get; private set; }

        internal Result(string json)
        {
            Success = true;
            JsonAsString = json;
        }

        internal Result(Error error)
        {
            Success = false;
            Error = error;
        }
    }
}

