using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.JSON.Transform.Definitions
{
    /// <summary>
    /// Input consist of parameters used in this Task.
    /// </summary>
    public class Input
    {
        /// <summary>
        /// Map input json in String or JToken type
        /// </summary>
        /// <example>{\"name\":\"veijo\"}</example>
        [DisplayFormat(DataFormatString = "Json")]
        [DefaultValue("{\"name\":\"veijo\"}")]
        public object InputJson { get; set; }

        /// <summary>
        /// JUST json map. See: https://github.com/WorkMaze/JUST.net#just
        /// </summary>
        /// <example>{\"firstName\":\"#valueof($.name)\"}</example>
        [DisplayFormat(DataFormatString = "Json")]
        [DefaultValue("{\"firstName\":\"#valueof($.name)\"}")]
        public string JsonMap { get; set; }
    }
}
