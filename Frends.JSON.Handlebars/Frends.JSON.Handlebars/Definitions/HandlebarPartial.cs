using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Frends.JSON.Handlebars.Definitions;

/// <summary>
/// HandlebarPartial values.
/// </summary>
public class HandlebarPartial
{
    /// <summary>
    /// Template name that exists in the HandlebarTemplate.
    /// </summary>
    /// <example>strongName</example>
    [DefaultValue("\"strongName\"")]
    public string TemplateName { get; set; }

    /// <summary>
    /// Partial template. This needs to be in expression mode. Using {{ }} in other modes breaks the task.
    /// </summary>
    /// <example>&lt;strong&gt;{{name}}&lt;/strong&gt;</example>
    [DefaultValue("\"<strong>{{name}}</strong>\"")]
    [DisplayFormat(DataFormatString = "Expression")]
    public string Template { get; set; }
}