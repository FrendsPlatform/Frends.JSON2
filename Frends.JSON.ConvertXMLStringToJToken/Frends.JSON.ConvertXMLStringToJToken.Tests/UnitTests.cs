using System;
using Frends.JSON.ConvertXMLStringToJToken.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Frends.JSON.ConvertXMLStringToJToken.Tests;

[TestClass]
public class UnitTests
{
    [TestMethod]
    public void ShouldConvertXmlStringToJToken()
    {
        var input = new Input()
        {
            XML = @"<?xml version='1.0' standalone='no'?>
             <root>
               <person id='1'>
                 <name>Alan</name>
                 <url>http://www.google.com</url>
               </person>
               <person id='2'>
                <name>Louis</name>
                 <url>http://www.yahoo.com</url>
              </person>
            </root>"
        };

        var result = JSON.ConvertXMLStringToJToken(input, new Options());
        Assert.IsTrue(result.Success);
        Assert.IsInstanceOfType(result.Jtoken, typeof(JObject));
    }

    [TestMethod]
    public void ShouldUseXsdToMapSingleElementAsArray()
    {
        var input = new Input()
        {
            XML = @"<?xml version='1.0' standalone='no'?>
             <root>
               <person id='1'>
                 <name>Alan</name>
               </person>
             </root>"
        };

        var options = new Options()
        {
            TypeCorrection = TypeCorrectionMode.Schema,
            XSD = @"<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema'>
                      <xs:element name='root'>
                        <xs:complexType>
                          <xs:sequence>
                            <xs:element name='person' maxOccurs='unbounded'>
                              <xs:complexType>
                                <xs:sequence>
                                  <xs:element name='name' type='xs:string' />
                                </xs:sequence>
                                <xs:attribute name='id' type='xs:string' />
                              </xs:complexType>
                            </xs:element>
                          </xs:sequence>
                        </xs:complexType>
                      </xs:element>
                    </xs:schema>"
        };

        var result = JSON.ConvertXMLStringToJToken(input, options);
        var root = ((JObject)result.Jtoken)["root"];

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(root);
        Assert.IsInstanceOfType(root["person"], typeof(JArray));

        var persons = root["person"] as JArray;

        Assert.IsNotNull(persons);

        Assert.AreEqual(1, persons.Count);
        Assert.AreEqual("Alan", persons[0]["name"]?.ToString());
    }

    [TestMethod]
    public void NoneMode_ShouldKeepValuesAsStrings()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                      <price xsi:type='float'>12.5</price>
                    </root>"
        };

        var result = JSON.ConvertXMLStringToJToken(input, new Options { TypeCorrection = TypeCorrectionMode.None });
        var price = ((JObject)result.Jtoken)["root"]?["price"];

        // Default behaviour is unchanged: the xsi:type wrapper and string value are preserved.
        Assert.IsInstanceOfType(price, typeof(JObject));
        Assert.AreEqual(JTokenType.String, price["#text"].Type);
        Assert.AreEqual("12.5", price["#text"].ToString());
    }

    [TestMethod]
    public void AttributesMode_ShouldConvertNumericAndBooleanValues()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                      <price xsi:type='float'>12.5</price>
                      <quantity xsi:type='int'>3</quantity>
                      <huge xsi:type='integer'>123456789012345678901234567890</huge>
                      <inStock xsi:type='boolean'>true</inStock>
                      <name>Widget</name>
                    </root>"
        };

        var result = JSON.ConvertXMLStringToJToken(input, new Options { TypeCorrection = TypeCorrectionMode.Attributes });
        var root = (JObject)((JObject)result.Jtoken)["root"];

        Assert.IsTrue(result.Success);
        Assert.AreEqual(JTokenType.Float, root["price"].Type);
        Assert.AreEqual(12.5, root["price"].Value<double>());
        Assert.AreEqual(JTokenType.Integer, root["quantity"].Type);
        Assert.AreEqual(3, root["quantity"].Value<int>());
        Assert.AreEqual(JTokenType.Integer, root["huge"].Type);
        Assert.AreEqual(JTokenType.Boolean, root["inStock"].Type);
        Assert.IsTrue(root["inStock"].Value<bool>());
        Assert.AreEqual(JTokenType.String, root["name"].Type);
    }

    [TestMethod]
    public void AttributesMode_ShouldKeepOtherAttributesWhileTypingText()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                      <price currency='EUR' xsi:type='float'>12.5</price>
                    </root>"
        };

        var result = JSON.ConvertXMLStringToJToken(input, new Options { TypeCorrection = TypeCorrectionMode.Attributes });
        var price = ((JObject)result.Jtoken)["root"]?["price"];

        // Element carries another attribute, so the wrapper is preserved but #text becomes a number.
        Assert.IsInstanceOfType(price, typeof(JObject));
        Assert.AreEqual("EUR", price["@currency"].ToString());
        Assert.IsNull(price["@xsi:type"]);
        Assert.AreEqual(JTokenType.Float, price["#text"].Type);
        Assert.AreEqual(12.5, price["#text"].Value<double>());
    }

    [TestMethod]
    public void AttributesMode_ShouldLeaveUnparseableValuesAsStrings()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                      <price xsi:type='float'>not-a-number</price>
                    </root>"
        };

        var result = JSON.ConvertXMLStringToJToken(input, new Options { TypeCorrection = TypeCorrectionMode.Attributes });
        var price = ((JObject)result.Jtoken)["root"]?["price"];

        Assert.IsInstanceOfType(price, typeof(JObject));
        Assert.AreEqual(JTokenType.String, price["#text"].Type);
        Assert.AreEqual("not-a-number", price["#text"].ToString());
    }

    [TestMethod]
    public void SchemaMode_ShouldConvertValuesUsingXsdTypes()
    {
        var input = new Input()
        {
            XML = @"<root>
                      <price>12.5</price>
                      <quantity>3</quantity>
                      <inStock>true</inStock>
                      <name>Widget</name>
                    </root>"
        };

        var options = new Options()
        {
            TypeCorrection = TypeCorrectionMode.Schema,
            XSD = @"<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema'>
                      <xs:element name='root'>
                        <xs:complexType>
                          <xs:sequence>
                            <xs:element name='price' type='xs:float' />
                            <xs:element name='quantity' type='xs:int' />
                            <xs:element name='inStock' type='xs:boolean' />
                            <xs:element name='name' type='xs:string' />
                          </xs:sequence>
                        </xs:complexType>
                      </xs:element>
                    </xs:schema>"
        };

        var result = JSON.ConvertXMLStringToJToken(input, options);
        var root = (JObject)((JObject)result.Jtoken)["root"];

        Assert.IsTrue(result.Success);
        Assert.AreEqual(JTokenType.Float, root["price"].Type);
        Assert.AreEqual(12.5, root["price"].Value<double>());
        Assert.AreEqual(JTokenType.Integer, root["quantity"].Type);
        Assert.AreEqual(JTokenType.Boolean, root["inStock"].Type);
        Assert.IsTrue(root["inStock"].Value<bool>());
        Assert.AreEqual(JTokenType.String, root["name"].Type);
    }

    [TestMethod]
    public void SchemaMode_ShouldConvertSingleElementArraysToTypedArrays()
    {
        var input = new Input()
        {
            XML = @"<root>
                      <score>9.5</score>
                    </root>"
        };

        var options = new Options()
        {
            TypeCorrection = TypeCorrectionMode.Schema,
            XSD = @"<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema'>
                      <xs:element name='root'>
                        <xs:complexType>
                          <xs:sequence>
                            <xs:element name='score' type='xs:double' maxOccurs='unbounded' />
                          </xs:sequence>
                        </xs:complexType>
                      </xs:element>
                    </xs:schema>"
        };

        var result = JSON.ConvertXMLStringToJToken(input, options);
        var scores = ((JObject)result.Jtoken)["root"]?["score"] as JArray;

        Assert.IsNotNull(scores);
        Assert.AreEqual(1, scores.Count);
        Assert.AreEqual(JTokenType.Float, scores[0].Type);
        Assert.AreEqual(9.5, scores[0].Value<double>());
    }

    [TestMethod]
    public void AttributesMode_ThrowAction_ShouldThrowOnUnparseableValue()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                      <price xsi:type='float'>not-a-number</price>
                    </root>"
        };

        var options = new Options
        {
            TypeCorrection = TypeCorrectionMode.Attributes,
            ActionOnBadValues = BadValueAction.Throw,
        };

        var ex = Assert.ThrowsException<FormatException>(
            () => JSON.ConvertXMLStringToJToken(input, options));

        StringAssert.Contains(ex.Message, "not-a-number");
        StringAssert.Contains(ex.Message, "float");
    }

    [TestMethod]
    public void AttributesMode_ThrowAction_ShouldNotThrowWhenAllValuesParse()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                      <price xsi:type='float'>12.5</price>
                      <inStock xsi:type='boolean'>true</inStock>
                    </root>"
        };

        var options = new Options
        {
            TypeCorrection = TypeCorrectionMode.Attributes,
            ActionOnBadValues = BadValueAction.Throw,
        };

        var result = JSON.ConvertXMLStringToJToken(input, options);
        var root = (JObject)((JObject)result.Jtoken)["root"];

        Assert.IsTrue(result.Success);
        Assert.AreEqual(JTokenType.Float, root["price"].Type);
        Assert.AreEqual(JTokenType.Boolean, root["inStock"].Type);
    }

    [TestMethod]
    public void AttributesMode_ThrowAction_ShouldThrowOnUnparseableBoolean()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                      <inStock xsi:type='boolean'>maybe</inStock>
                    </root>"
        };

        var options = new Options
        {
            TypeCorrection = TypeCorrectionMode.Attributes,
            ActionOnBadValues = BadValueAction.Throw,
        };

        var ex = Assert.ThrowsException<FormatException>(
            () => JSON.ConvertXMLStringToJToken(input, options));

        StringAssert.Contains(ex.Message, "maybe");
        StringAssert.Contains(ex.Message, "boolean");
    }

    [TestMethod]
    public void AttributesMode_IgnoreAction_IsDefaultAndKeepsUnparseableAsString()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                      <price xsi:type='float'>not-a-number</price>
                    </root>"
        };

        var options = new Options
        {
            TypeCorrection = TypeCorrectionMode.Attributes,
        };

        Assert.AreEqual(BadValueAction.Ignore, options.ActionOnBadValues);

        var result = JSON.ConvertXMLStringToJToken(input, options);
        var price = ((JObject)result.Jtoken)["root"]?["price"];

        Assert.IsInstanceOfType(price, typeof(JObject));
        Assert.AreEqual(JTokenType.String, price["#text"].Type);
        Assert.AreEqual("not-a-number", price["#text"].ToString());
    }

    [TestMethod]
    public void AttributesMode_ThrowAction_ShouldNotThrowOnUnknownXsiType()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                      <data xsi:type='hexBinary'>DEADBEEF</data>
                    </root>"
        };

        var options = new Options
        {
            TypeCorrection = TypeCorrectionMode.Attributes,
            ActionOnBadValues = BadValueAction.Throw,
        };

        // xsi:types we don't know how to convert (e.g. hexBinary) are not treated as "bad" —
        // they're simply left alone regardless of the action setting.
        var result = JSON.ConvertXMLStringToJToken(input, options);

        Assert.IsTrue(result.Success);
    }

    [TestMethod]
    public void SchemaMode_WithoutXsd_ShouldNoOpAndKeepStrings()
    {
        var input = new Input()
        {
            XML = "<root><price>12.5</price></root>"
        };

        // Schema mode without an XSD is a graceful no-op (not an error), so migrated
        // processes that land in Schema mode without a schema keep their previous output.
        var result = JSON.ConvertXMLStringToJToken(input, new Options { TypeCorrection = TypeCorrectionMode.Schema });
        var price = ((JObject)result.Jtoken)["root"]?["price"];

        Assert.IsTrue(result.Success);
        Assert.AreEqual(JTokenType.String, price.Type);
        Assert.AreEqual("12.5", price.ToString());
    }
}
