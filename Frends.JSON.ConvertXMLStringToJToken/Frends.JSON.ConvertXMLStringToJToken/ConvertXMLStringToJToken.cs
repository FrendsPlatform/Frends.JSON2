using Frends.JSON.ConvertXMLStringToJToken.Definitions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Frends.JSON.ConvertXMLStringToJToken;

/// <summary>
/// JSON Task.
/// </summary>
public class JSON
{
    private const string JsonNamespace = "http://james.newtonking.com/projects/json";
    private const string XsiNamespace = "http://www.w3.org/2001/XMLSchema-instance";

    private const string TextPropertyName = "#text";
    private const string XsiTypePropertyName = "@xsi:type";

    /// <summary>
    /// Maps an XSD type local name to a parser producing a native JSON value, or null when unparseable.
    /// </summary>
    private static readonly Dictionary<string, Func<string, JValue>> TypeConverters =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["float"] = ParseFloatingPoint,
            ["double"] = ParseFloatingPoint,
            ["decimal"] = ParseDecimal,
            ["int"] = ParseInteger,
            ["integer"] = ParseInteger,
            ["long"] = ParseInteger,
            ["short"] = ParseInteger,
            ["byte"] = ParseInteger,
            ["unsignedInt"] = ParseInteger,
            ["unsignedLong"] = ParseInteger,
            ["unsignedShort"] = ParseInteger,
            ["unsignedByte"] = ParseInteger,
            ["nonNegativeInteger"] = ParseInteger,
            ["positiveInteger"] = ParseInteger,
            ["nonPositiveInteger"] = ParseInteger,
            ["negativeInteger"] = ParseInteger,
            ["boolean"] = ParseBoolean,
        };

    /// <summary>
    /// Convert XML string to JToken.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.JSON.ConvertXMLStringToJToken)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <param name="options">Optional parameters</param>
    /// <returns>Object { bool Success, object Jtoken }</returns>
    public static Result ConvertXMLStringToJToken([PropertyTab] Input input, [PropertyTab] Options options)
    {
        options ??= new Options();

        // Schema mode without an XSD is treated as a no-op rather than an error so that
        // processes migrated into Schema mode without a schema keep their previous output.
        var useSchema = options.TypeCorrection == TypeCorrectionMode.Schema
            && !string.IsNullOrWhiteSpace(options.XSD);

        var doc = useSchema
            ? LoadXmlDocumentWithSchemaHints(input.XML, options.XSD, options.TypeCorrection)
            : LoadXmlDocument(input.XML);

        var jsonString = JsonConvert.SerializeXmlNode(doc);
        var token = JToken.Parse(jsonString);

        if (options.TypeCorrection != TypeCorrectionMode.None)
            CorrectTypes(token, options.ActionOnBadValues);

        return new Result(true, token);
    }

    private static XmlDocument LoadXmlDocument(string xml)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        return doc;
    }

    private static XmlDocument LoadXmlDocumentWithSchemaHints(string xml, string xsd, TypeCorrectionMode typeCorrection)
    {
        var schemaSet = CreateSchemaSet(xsd);

        var xDocument = XDocument.Parse(xml);

        xDocument.Validate(
            schemaSet,
            (sender, args) =>
            {
                throw new XmlSchemaValidationException(
                    $"XML schema validation failed: {args.Message}",
                    args.Exception);
            },
            true);

        AddSchemaHints(xDocument, typeCorrection);

        var xmlDocument = new XmlDocument();

        using var reader = xDocument.CreateReader();
        xmlDocument.Load(reader);

        return xmlDocument;
    }

    private static XmlSchemaSet CreateSchemaSet(string xsd)
    {
        var schemaSet = new XmlSchemaSet();
        using var schemaReader = XmlReader.Create(new StringReader(xsd));
        schemaSet.Add(null, schemaReader);
        schemaSet.Compile();
        return schemaSet;
    }

    /// <summary>
    /// Walks the validated document once, tagging elements that the schema declares as
    /// repeatable with json:Array, and (when requested) numeric/boolean elements with xsi:type
    /// so they can be converted to native JSON types after serialization.
    /// </summary>
    private static void AddSchemaHints(XDocument document, TypeCorrectionMode typeCorrection)
    {
        if (document.Root == null)
            return;

        XNamespace jsonNs = JsonNamespace;
        XNamespace xsiNs = XsiNamespace;
        var hasArray = false;
        var hasType = false;

        foreach (var element in document.Root.DescendantsAndSelf())
        {
            var schemaInfo = element.GetSchemaInfo();
            var schemaElement = schemaInfo?.SchemaElement;

            if (schemaElement?.MaxOccurs > 1m)
            {
                element.SetAttributeValue(jsonNs + "Array", "true");
                hasArray = true;
            }

            if (typeCorrection == TypeCorrectionMode.Schema)
            {
                var typeName = GetConvertibleTypeName(schemaInfo?.SchemaType);
                if (typeName != null)
                {
                    element.SetAttributeValue(xsiNs + "type", typeName);
                    hasType = true;
                }
            }
        }

        if (hasArray && !IsNamespaceDeclared(document.Root, JsonNamespace))
            document.Root.SetAttributeValue(XNamespace.Xmlns + "json", JsonNamespace);

        // Pin the "xsi" prefix so injected type hints serialize as @xsi:type rather than an
        // auto-generated prefix that the post-serialization conversion would not recognise.
        if (hasType && !IsNamespaceDeclared(document.Root, XsiNamespace))
            document.Root.SetAttributeValue(XNamespace.Xmlns + "xsi", XsiNamespace);
    }

    private static bool IsNamespaceDeclared(XElement root, string namespaceName)
    {
        return root.Attributes().Any(a => a.IsNamespaceDeclaration && a.Value == namespaceName);
    }

    /// <summary>
    /// Returns a canonical XSD type name (matching <see cref="TypeConverters"/>) for numeric/boolean
    /// schema types, or null for types that should remain strings.
    /// </summary>
    private static string GetConvertibleTypeName(XmlSchemaType schemaType)
    {
        return schemaType?.TypeCode switch
        {
            XmlTypeCode.Float => "float",
            XmlTypeCode.Double => "double",
            XmlTypeCode.Decimal => "decimal",
            XmlTypeCode.Boolean => "boolean",
            XmlTypeCode.Integer
                or XmlTypeCode.NonPositiveInteger
                or XmlTypeCode.NegativeInteger
                or XmlTypeCode.Long
                or XmlTypeCode.Int
                or XmlTypeCode.Short
                or XmlTypeCode.Byte
                or XmlTypeCode.NonNegativeInteger
                or XmlTypeCode.UnsignedLong
                or XmlTypeCode.UnsignedInt
                or XmlTypeCode.UnsignedShort
                or XmlTypeCode.UnsignedByte
                or XmlTypeCode.PositiveInteger => "integer",
            _ => null,
        };
    }

    /// <summary>
    /// Recursively converts values carrying an xsi:type hint into native JSON numbers/booleans,
    /// removing the consumed xsi:type attribute and collapsing the wrapper object when only the
    /// converted value remains.
    /// </summary>
    private static void CorrectTypes(JToken token, BadValueAction actionOnBadValues)
    {
        switch (token)
        {
            case JArray array:
                foreach (var item in array.Children().ToList())
                    CorrectTypes(item, actionOnBadValues);
                break;

            case JObject obj:
                foreach (var value in obj.PropertyValues().ToList())
                    CorrectTypes(value, actionOnBadValues);
                ApplyTypeHint(obj, actionOnBadValues);
                break;
        }
    }

    private static void ApplyTypeHint(JObject obj, BadValueAction actionOnBadValues)
    {
        var typeProperty = obj.Property(XsiTypePropertyName, StringComparison.Ordinal);
        if (typeProperty == null)
            return;

        var typeName = LocalName(typeProperty.Value.ToString());
        if (!TypeConverters.TryGetValue(typeName, out var convert))
            return;

        var textProperty = obj.Property(TextPropertyName, StringComparison.Ordinal);
        if (textProperty == null || textProperty.Value.Type != JTokenType.String)
            return;

        var rawValue = textProperty.Value.ToString();
        var converted = convert(rawValue);
        if (converted == null)
        {
            if (actionOnBadValues == BadValueAction.Throw)
                throw new FormatException(
                    $"Value '{rawValue}' on <{obj.Path}> could not be converted to '{typeName}'.");
            return;
        }

        typeProperty.Remove();

        var remaining = obj.Properties().ToList();
        var onlyText = remaining.Count == 1 && remaining[0].Name == TextPropertyName;
        var onlyTextAndXsiNs = remaining.Count == 2 &&
            remaining.Any(p => p.Name == TextPropertyName) &&
            remaining.Any(IsXsiNamespaceDeclaration);

        if ((onlyText || onlyTextAndXsiNs) && obj.Parent != null)
            obj.Replace(converted);
        else
            textProperty.Value = converted;
    }

    private static bool IsXsiNamespaceDeclaration(JProperty property)
    {
        return property.Name.StartsWith("@xmlns", StringComparison.Ordinal) &&
            string.Equals(property.Value.ToString(), XsiNamespace, StringComparison.Ordinal);
    }

    private static string LocalName(string value)
    {
        var index = value.IndexOf(':');
        return index >= 0 ? value[(index + 1)..] : value;
    }

    private static JValue ParseFloatingPoint(string value)
    {
        switch (value.Trim())
        {
            case "INF": return new JValue(double.PositiveInfinity);
            case "-INF": return new JValue(double.NegativeInfinity);
            case "NaN": return new JValue(double.NaN);
        }

        return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
            ? new JValue(parsed)
            : null;
    }

    private static JValue ParseDecimal(string value)
    {
        return decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
            ? new JValue(parsed)
            : null;
    }

    private static JValue ParseInteger(string value)
    {
        if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedLong))
            return new JValue(parsedLong);

        return BigInteger.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedBig)
            ? new JValue(parsedBig)
            : null;
    }

    private static JValue ParseBoolean(string value)
    {
        switch (value.Trim())
        {
            case "true":
            case "1":
                return new JValue(true);
            case "false":
            case "0":
                return new JValue(false);
            default:
                return null;
        }
    }
}
