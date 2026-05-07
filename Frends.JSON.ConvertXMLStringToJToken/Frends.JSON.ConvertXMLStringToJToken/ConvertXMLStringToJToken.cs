using Frends.JSON.ConvertXMLStringToJToken.Definitions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.IO;
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

    /// <summary>
    /// Convert XML string to JToken.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.JSON.ConvertXMLStringToJToken)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <returns>Object { bool Success, object Jtoken }</returns>
    public static Result ConvertXMLStringToJToken([PropertyTab] Input input)
    {
        var doc = string.IsNullOrWhiteSpace(input.XSD)
            ? LoadXmlDocument(input.XML)
            : LoadXmlDocumentWithSchemaHints(input.XML, input.XSD);

        var jsonString = JsonConvert.SerializeXmlNode(doc);
        return new Result(true, JToken.Parse(jsonString));
    }

    private static XmlDocument LoadXmlDocument(string xml)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        return doc;
    }

    private static XmlDocument LoadXmlDocumentWithSchemaHints(string xml, string xsd)
    {
        var schemaSet = CreateSchemaSet(xsd);
        ValidateXmlWithSchema(xml, schemaSet);

        var xDocument = XDocument.Parse(xml);
        // Populate schema info to map XML elements to JSON arrays based on schema occurrences.
        xDocument.Validate(schemaSet, null, true);
        AddJsonArrayAttributesFromSchema(xDocument);

        return LoadXmlDocument(xDocument.ToString(SaveOptions.DisableFormatting));
    }

    private static XmlSchemaSet CreateSchemaSet(string xsd)
    {
        var schemaSet = new XmlSchemaSet();
        using var schemaReader = XmlReader.Create(new StringReader(xsd));
        schemaSet.Add(null, schemaReader);
        schemaSet.Compile();
        return schemaSet;
    }

    private static void ValidateXmlWithSchema(string xml, XmlSchemaSet schemaSet)
    {
        var settings = new XmlReaderSettings
        {
            ValidationType = ValidationType.Schema,
            Schemas = schemaSet
        };

        settings.ValidationEventHandler += (_, args) =>
            throw new XmlSchemaValidationException($"XML schema validation failed: {args.Message}", args.Exception);

        using var xmlReader = XmlReader.Create(new StringReader(xml), settings);
        while (xmlReader.Read())
        {
            // Read whole document to trigger schema validation.
        }
    }

    private static void AddJsonArrayAttributesFromSchema(XDocument document)
    {
        if (document.Root == null) return;

        XNamespace jsonNs = JsonNamespace;
        var hasArray = false;

        foreach (var element in document.Root.DescendantsAndSelf())
        {
            var schemaElement = element.GetSchemaInfo()?.SchemaElement;
            if (schemaElement?.MaxOccurs > 1m)
            {
                element.SetAttributeValue(jsonNs + "Array", "true");
                hasArray = true;
            }
        }

        if (hasArray && document.Root.GetPrefixOfNamespace(jsonNs) == null)
            document.Root.SetAttributeValue(XNamespace.Xmlns + "json", JsonNamespace);
    }
}
