using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.JSON.ConvertXMLStringToJToken.Definitions;

/// <summary>
/// Controls how XML values are mapped to native JSON types.
/// </summary>
public enum TypeCorrectionMode
{
    /// <summary>
    /// No type correction. Every value is emitted as a JSON string (default, backwards compatible).
    /// </summary>
    None,

    /// <summary>
    /// Read inline xsi:type attributes from the XML (e.g. xsi:type="float") to convert
    /// numeric and boolean values into native JSON types.
    /// </summary>
    Attributes,

    /// <summary>
    /// Derive value types from the provided XSD schema (xs:float, xs:int, xs:boolean, ...)
    /// to convert numeric and boolean values into native JSON types. Requires an XSD.
    /// </summary>
    Schema,
}

/// <summary>
/// Optional parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// Whether and how to convert string values into native JSON numbers and booleans.
    /// None leaves all values as strings (default). Attributes reads inline xsi:type hints
    /// from the XML. Schema derives the types from the supplied XSD.
    /// </summary>
    /// <example>TypeCorrectionMode.Attributes</example>
    [DefaultValue(TypeCorrectionMode.None)]
    public TypeCorrectionMode TypeCorrection { get; set; } = TypeCorrectionMode.None;

    /// <summary>
    /// XSD schema used to validate the XML and to derive value types and array mapping.
    /// Only used (and shown) when TypeCorrection is Schema.
    /// </summary>
    /// <example>
    /// &lt;xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema'&gt;...&lt;/xs:schema&gt;
    /// </example>
    [UIHint(nameof(TypeCorrection), "", TypeCorrectionMode.Schema)]
    public string XSD { get; set; }
}
