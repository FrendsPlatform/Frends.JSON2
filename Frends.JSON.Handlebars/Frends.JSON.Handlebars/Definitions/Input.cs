﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Frends.JSON.Handlebars.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// JSON input needs to be of type string or JToken.
    /// </summary>
    /// <example>{ \"title\":\"Mr.\", \"name\":\"Foo\" }</example>
    [DisplayFormat(DataFormatString = "Json")]
    public dynamic Json { get; set; }

    /// <summary>
    /// Template for handlebars. 
    /// > indicates a partial. 
    /// This needs to be in expression mode. 
    /// Using {{ }} in other modes breaks the task.
    /// </summary>
    /// <example>&lt;xml&gt; {{title}} {{> strongName}} &lt;/xml&gt;</example>
    [DefaultValue("\"<xml> {{title}} {{> strongName}} </xml>\"")]
    [DisplayFormat(DataFormatString = "Expression")]
    public string HandlebarTemplate { get; set; }

    /// <summary>
    /// Partials for template.
    /// </summary>
    /// <example>[ strongName, &lt;strong&gt;{{name}}&lt;/strong&gt; ]</example>
    public HandlebarPartial[] HandlebarPartials { get; set; }
}