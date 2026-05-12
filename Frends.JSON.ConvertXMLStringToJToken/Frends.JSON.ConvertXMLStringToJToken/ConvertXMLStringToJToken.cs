using Frends.JSON.ConvertXMLStringToJToken.Definitions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.IO;
using System.Linq;
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

        AddJsonArrayAttributesFromSchema(xDocument);

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

    private static void AddJsonArrayAttributesFromSchema(XDocument document)
    {
        if (document.Root == null)
            return;

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

        var existing = document.Root.Attributes()
            .FirstOrDefault(a =>
                a.IsNamespaceDeclaration &&
                a.Value == JsonNamespace);

        if (hasArray && existing == null)
        {
            document.Root.SetAttributeValue(
                XNamespace.Xmlns + "json",
                JsonNamespace);
        }
    }
}
