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
    private const string XmlnsPrefixSeparator = "@xmlns:";

    private static readonly IReadOnlyDictionary<string, string> EmptyXmlnsScope =
        new Dictionary<string, string>(0, StringComparer.Ordinal);

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

        // Attributes always runs; Schema mode only runs when it has an XSD to derive types from,
        // so picking Schema with a blank XSD stays a true no-op even if the input XML happens to
        // carry inline xsi:type attributes.
        var effectiveMode = useSchema
            ? TypeCorrectionMode.Schema
            : options.TypeCorrection == TypeCorrectionMode.Attributes
                ? TypeCorrectionMode.Attributes
                : TypeCorrectionMode.None;

        if (effectiveMode != TypeCorrectionMode.None)
        {
            CorrectTypes(token, options.ActionOnBadValues, effectiveMode);

            // Schema mode injects an xmlns:xsi declaration to carry the derived xsi:type hints
            // (see AddSchemaHints). Once CorrectTypes has consumed those hints the declaration is
            // orphaned, so strip it (and any other now-unused xsi declaration) from the output.
            RemoveUnusedXsiNamespaceDeclarations(token);
        }

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

        // In Schema mode the XSD is the single source of truth; pull any author-written
        // xsi:type off the document up front so it can't trip the validator (an unqualified
        // xsi:type='int' is otherwise rejected before we ever get to AddSchemaHints) or shadow
        // a schema-derived hint.
        RemoveInlineXsiTypeAttributes(xDocument);

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

    /// <summary>
    /// Removes any xsi:type attribute (under any prefix bound to the XML Schema Instance
    /// namespace) from every element in the document.
    /// </summary>
    private static void RemoveInlineXsiTypeAttributes(XDocument document)
    {
        if (document.Root == null)
            return;

        XNamespace xsiNs = XsiNamespace;
        var xsiType = xsiNs + "type";

        foreach (var element in document.Root.DescendantsAndSelf())
            element.Attribute(xsiType)?.Remove();
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
    private static void CorrectTypes(JToken token, BadValueAction actionOnBadValues, TypeCorrectionMode mode)
    {
        CorrectTypes(token, actionOnBadValues, mode, EmptyXmlnsScope);
    }

    private static void CorrectTypes(
        JToken token,
        BadValueAction actionOnBadValues,
        TypeCorrectionMode mode,
        IReadOnlyDictionary<string, string> xmlnsScope)
    {
        switch (token)
        {
            case JArray array:
                foreach (var item in array.Children().ToList())
                    CorrectTypes(item, actionOnBadValues, mode, xmlnsScope);
                break;

            case JObject obj:
                var childScope = ExtendXmlnsScope(obj, xmlnsScope);
                foreach (var value in obj.PropertyValues().ToList())
                    CorrectTypes(value, actionOnBadValues, mode, childScope);

                // xsi:nil handling runs first so it can collapse the wrapper to a default/null
                // (or strip the attribute so ApplyTypeHint sees a clean element).
                if (ApplyXsiNil(obj, mode, childScope))
                    break;

                ApplyTypeHint(obj, actionOnBadValues, childScope);
                break;
        }
    }

    /// <summary>
    /// Extends an xmlns prefix-to-namespace map with any @xmlns:* declarations carried on this
    /// JObject, so that nested elements can resolve type-hint prefixes (e.g. xmlns:i bound here,
    /// i:type used on a descendant). Inner declarations shadow outer ones, matching XML scope.
    /// </summary>
    private static IReadOnlyDictionary<string, string> ExtendXmlnsScope(
        JObject obj,
        IReadOnlyDictionary<string, string> parentScope)
    {
        Dictionary<string, string> extended = null;

        foreach (var prop in obj.Properties())
        {
            if (!prop.Name.StartsWith(XmlnsPrefixSeparator, StringComparison.Ordinal))
                continue;

            extended ??= new Dictionary<string, string>(parentScope, StringComparer.Ordinal);
            var prefix = prop.Name.Substring(XmlnsPrefixSeparator.Length);
            extended[prefix] = prop.Value.ToString();
        }

        return extended ?? parentScope;
    }

    private static void ApplyTypeHint(
        JObject obj,
        BadValueAction actionOnBadValues,
        IReadOnlyDictionary<string, string> xmlnsScope)
    {
        var typeProperty = FindXsiTypeProperty(obj, xmlnsScope);
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

    private static JProperty FindXsiTypeProperty(JObject obj, IReadOnlyDictionary<string, string> xmlnsScope) =>
        FindXsiNamespacedAttribute(obj, "type", xmlnsScope);

    private static JProperty FindXsiNilProperty(JObject obj, IReadOnlyDictionary<string, string> xmlnsScope) =>
        FindXsiNamespacedAttribute(obj, "nil", xmlnsScope);

    /// <summary>
    /// Finds the first attribute on this JObject named "@&lt;prefix&gt;:&lt;localName&gt;" where
    /// &lt;prefix&gt; is bound (in the active xmlns scope) to the XML Schema Instance namespace.
    /// Newtonsoft preserves the source prefix, so authors using xmlns:i (instead of the
    /// conventional xsi) still get their type/nil hints honoured.
    /// </summary>
    private static JProperty FindXsiNamespacedAttribute(
        JObject obj,
        string localName,
        IReadOnlyDictionary<string, string> xmlnsScope)
    {
        foreach (var prop in obj.Properties())
        {
            if (prop.Name.Length < 2 || prop.Name[0] != '@')
                continue;

            var colonIndex = prop.Name.IndexOf(':');
            if (colonIndex < 0)
                continue;

            var local = prop.Name.Substring(colonIndex + 1);
            if (!string.Equals(local, localName, StringComparison.Ordinal))
                continue;

            var prefix = prop.Name.Substring(1, colonIndex - 1);
            if (xmlnsScope.TryGetValue(prefix, out var ns) &&
                string.Equals(ns, XsiNamespace, StringComparison.Ordinal))
                return prop;
        }

        return null;
    }

    /// <summary>
    /// Applies xsi:nil semantics. Returns true when the JObject was replaced/finalised and no
    /// further type-hint processing is needed on it.
    /// <para>nil="true" empty → JSON null (replacing the wrapper).
    /// nil="true" with content → strip the attribute, let content flow through.
    /// nil="false" empty → coerce to default(T) (T from xsi:type, or "" when Schema mode
    /// validated the element without providing a convertible type; Attributes mode without
    /// xsi:type is left alone).
    /// nil="false" with content → strip the attribute, let content flow through.</para>
    /// </summary>
    private static bool ApplyXsiNil(
        JObject obj,
        TypeCorrectionMode mode,
        IReadOnlyDictionary<string, string> xmlnsScope)
    {
        var nilProperty = FindXsiNilProperty(obj, xmlnsScope);
        if (nilProperty == null)
            return false;

        var nilFlag = ParseNilFlag(nilProperty.Value.ToString());
        if (nilFlag == null)
            return false;

        var textProperty = obj.Property(TextPropertyName, StringComparison.Ordinal);
        if (HasContent(obj, textProperty))
        {
            // Content wins — drop the nil attribute. If the wrapper now has nothing else
            // meaningful to carry (no xsi:type for further conversion, no other attributes),
            // collapse it to the bare text value so the shape matches an authored element
            // that never had xsi:nil.
            nilProperty.Remove();

            var typeProperty = FindXsiTypeProperty(obj, xmlnsScope);
            if (typeProperty == null && textProperty != null &&
                obj.Parent != null && OnlyTextOrNamespaceDeclarations(obj))
            {
                obj.Replace(textProperty.Value);
                return true;
            }

            return false;
        }

        // Empty element — handle the nil flag.
        if (nilFlag == true)
        {
            nilProperty.Remove();
            var typePropertyForNil = FindXsiTypeProperty(obj, xmlnsScope);
            typePropertyForNil?.Remove();

            if (obj.Parent != null)
                obj.Replace(JValue.CreateNull());
            else
                ReplaceOrClearWrapper(obj, textProperty, JValue.CreateNull());
            return true;
        }

        // nil = false on an empty element: coerce to default(T), but only when we actually
        // have type info to derive T from.
        var typePropertyForDefault = FindXsiTypeProperty(obj, xmlnsScope);
        JValue defaultValue;

        if (typePropertyForDefault != null)
        {
            defaultValue = DefaultValueForTypeName(LocalName(typePropertyForDefault.Value.ToString()));
            typePropertyForDefault.Remove();
        }
        else if (mode == TypeCorrectionMode.Schema)
        {
            // The schema validated this element; a missing xsi:type means the schema's type is
            // a non-convertible one (xs:string or similar). Default to an empty string.
            defaultValue = new JValue(string.Empty);
        }
        else
        {
            // Attributes mode with no inline xsi:type: per design, only act when type info is
            // present. Leave xsi:nil in place so the caller can still see the marker.
            return false;
        }

        nilProperty.Remove();
        ReplaceOrClearWrapper(obj, textProperty, defaultValue);
        return true;
    }

    private static bool OnlyTextOrNamespaceDeclarations(JObject obj)
    {
        foreach (var prop in obj.Properties())
        {
            if (prop.Name == TextPropertyName)
                continue;
            if (prop.Name.StartsWith("@xmlns", StringComparison.Ordinal))
                continue;
            return false;
        }
        return true;
    }

    private static bool? ParseNilFlag(string value)
    {
        var trimmed = value?.Trim() ?? string.Empty;

        if (trimmed == "1" || string.Equals(trimmed, "true", StringComparison.OrdinalIgnoreCase))
            return true;

        if (trimmed == "0" || string.Equals(trimmed, "false", StringComparison.OrdinalIgnoreCase))
            return false;

        return null;
    }

    private static bool HasContent(JObject obj, JProperty textProperty)
    {
        if (textProperty != null && textProperty.Value.Type == JTokenType.String &&
            !string.IsNullOrEmpty(textProperty.Value.ToString()))
            return true;

        foreach (var prop in obj.Properties())
        {
            if (prop.Name.Length == 0 || prop.Name[0] == '@' || prop.Name == TextPropertyName)
                continue;

            return true;
        }

        return false;
    }

    /// <summary>
    /// When the wrapper's only remaining properties are namespace declarations, collapse it to
    /// the new scalar value. Otherwise preserve the wrapper (other attributes are still
    /// meaningful) and assign the value to #text.
    /// </summary>
    private static void ReplaceOrClearWrapper(JObject obj, JProperty existingTextProperty, JValue newValue)
    {
        var canCollapse = obj.Parent != null &&
            obj.Properties().All(p => p.Name.StartsWith("@xmlns", StringComparison.Ordinal));

        if (canCollapse)
        {
            obj.Replace(newValue);
            return;
        }

        if (existingTextProperty != null)
            existingTextProperty.Value = newValue;
        else
            obj[TextPropertyName] = newValue;
    }

    /// <summary>
    /// Mirrors C# default(T) for the recognized XSD-ish type names; unknown types fall back to
    /// an empty string so callers always get a usable JSON value.
    /// </summary>
    private static JValue DefaultValueForTypeName(string typeName)
    {
        if (string.IsNullOrEmpty(typeName) || !TypeConverters.ContainsKey(typeName))
            return new JValue(string.Empty);

        return typeName.ToLowerInvariant() switch
        {
            "float" or "double" => new JValue(0d),
            "decimal" => new JValue(0m),
            "boolean" => new JValue(false),
            _ => new JValue(0L),
        };
    }

    /// <summary>
    /// Removes XML Schema Instance namespace declarations (e.g. @xmlns:xsi) that no surviving
    /// xsi:-prefixed attribute still references. Declarations bound to a prefix that is still in
    /// use (e.g. an unconverted xsi:type or a retained xsi:nil) are left intact.
    /// </summary>
    private static void RemoveUnusedXsiNamespaceDeclarations(JToken token)
    {
        switch (token)
        {
            case JArray array:
                foreach (var item in array.Children().ToList())
                    RemoveUnusedXsiNamespaceDeclarations(item);
                break;

            case JObject obj:
                var declarations = obj.Properties()
                    .Where(p => p.Name.StartsWith(XmlnsPrefixSeparator, StringComparison.Ordinal)
                        && IsXsiNamespaceDeclaration(p))
                    .ToList();

                foreach (var declaration in declarations)
                {
                    var prefix = declaration.Name.Substring(XmlnsPrefixSeparator.Length);
                    if (!IsPrefixedAttributeUsed(obj, "@" + prefix + ":"))
                        declaration.Remove();
                }

                foreach (var value in obj.PropertyValues().ToList())
                    RemoveUnusedXsiNamespaceDeclarations(value);
                break;
        }
    }

    /// <summary>
    /// Returns true when any attribute named "&lt;marker&gt;..." (e.g. "@xsi:") appears on this
    /// token or anywhere in its subtree — the scope over which a namespace declaration applies.
    /// </summary>
    private static bool IsPrefixedAttributeUsed(JToken token, string marker)
    {
        switch (token)
        {
            case JObject obj:
                foreach (var prop in obj.Properties())
                {
                    if (prop.Name.StartsWith(marker, StringComparison.Ordinal))
                        return true;
                    if (IsPrefixedAttributeUsed(prop.Value, marker))
                        return true;
                }
                return false;

            case JArray array:
                foreach (var item in array.Children())
                    if (IsPrefixedAttributeUsed(item, marker))
                        return true;
                return false;

            default:
                return false;
        }
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
        var trimmed = value.Trim();

        if (trimmed == "1" || string.Equals(trimmed, "true", StringComparison.OrdinalIgnoreCase))
            return new JValue(true);

        if (trimmed == "0" || string.Equals(trimmed, "false", StringComparison.OrdinalIgnoreCase))
            return new JValue(false);

        return null;
    }
}
