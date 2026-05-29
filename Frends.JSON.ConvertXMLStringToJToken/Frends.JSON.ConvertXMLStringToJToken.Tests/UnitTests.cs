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
        Assert.IsTrue(result.Success);

        var price = ((JObject)result.Jtoken)["root"]?["price"] as JObject;

        // Default behaviour is unchanged: the xsi:type wrapper and string value are preserved.
        Assert.IsNotNull(price);
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
    public void AttributesMode_BooleanParsing_ShouldBeCaseSensitive()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                      <lower xsi:type='boolean'>true</lower>
                      <upper xsi:type='boolean'>TRUE</upper>
                      <camel xsi:type='boolean'>True</camel>
                    </root>"
        };

        var result = JSON.ConvertXMLStringToJToken(input, new Options { TypeCorrection = TypeCorrectionMode.Attributes });
        var root = (JObject)((JObject)result.Jtoken)["root"];

        Assert.IsTrue(result.Success);
        Assert.AreEqual(JTokenType.Boolean, root["lower"].Type);
        Assert.IsTrue(root["lower"].Value<bool>());

        Assert.AreEqual(JTokenType.Boolean, root["upper"].Type);
        Assert.IsTrue(root["upper"].Value<bool>());

        Assert.AreEqual(JTokenType.Boolean, root["camel"].Type);
        Assert.IsTrue(root["camel"].Value<bool>());
    }

    [TestMethod]
    public void AttributesMode_ShouldHonourAliasedXsiNamespacePrefix()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:i='http://www.w3.org/2001/XMLSchema-instance'>
                      <price i:type='float'>12.5</price>
                      <inStock i:type='boolean'>true</inStock>
                    </root>"
        };

        // Author used 'i' instead of the conventional 'xsi' prefix; conversion must still fire
        // because the prefix is resolved against the XML Schema Instance namespace URI.
        var result = JSON.ConvertXMLStringToJToken(input, new Options { TypeCorrection = TypeCorrectionMode.Attributes });
        var root = (JObject)((JObject)result.Jtoken)["root"];

        Assert.IsTrue(result.Success);
        Assert.AreEqual(JTokenType.Float, root["price"].Type);
        Assert.AreEqual(12.5, root["price"].Value<double>());
        Assert.AreEqual(JTokenType.Boolean, root["inStock"].Type);
        Assert.IsTrue(root["inStock"].Value<bool>());
    }

    [TestMethod]
    public void AttributesMode_ShouldIgnoreTypePropertyWhenPrefixNotBoundToXsiNamespace()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:cust='http://example.com/custom'>
                      <price cust:type='float'>12.5</price>
                    </root>"
        };

        // 'cust' is bound to an unrelated namespace, so cust:type is just an arbitrary attribute
        // and must not trigger conversion.
        var result = JSON.ConvertXMLStringToJToken(input, new Options { TypeCorrection = TypeCorrectionMode.Attributes });
        var price = ((JObject)result.Jtoken)["root"]?["price"];

        Assert.IsInstanceOfType(price, typeof(JObject));
        Assert.AreEqual("float", price["@cust:type"]?.ToString());
        Assert.AreEqual(JTokenType.String, price["#text"].Type);
        Assert.AreEqual("12.5", price["#text"].ToString());
    }

    [TestMethod]
    public void AttributesMode_ShouldResolvePrefixDeclaredOnInnerElement()
    {
        var input = new Input()
        {
            XML = @"<root>
                      <wrapper xmlns:i='http://www.w3.org/2001/XMLSchema-instance'>
                        <price i:type='float'>12.5</price>
                      </wrapper>
                    </root>"
        };

        // The xsi-equivalent prefix is bound on <wrapper>, not on the root — the scope must
        // still be visible to the inner <price> element when we resolve i:type.
        var result = JSON.ConvertXMLStringToJToken(input, new Options { TypeCorrection = TypeCorrectionMode.Attributes });
        var price = ((JObject)result.Jtoken)["root"]?["wrapper"]?["price"];

        Assert.AreEqual(JTokenType.Float, price.Type);
        Assert.AreEqual(12.5, price.Value<double>());
    }

    [TestMethod]
    public void NoneMode_ShouldNotTouchXsiNilWrappers()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                      <fax xsi:nil='true'/>
                      <phone xsi:nil='false'/>
                    </root>"
        };

        // In None mode the task does no type/nullability processing, so Newtonsoft's raw
        // shape (wrapper objects holding @xsi:nil) is preserved verbatim. Anyone relying on
        // the pre-2.0.0 output stays unaffected by the xsi:nil feature.
        var result = JSON.ConvertXMLStringToJToken(input, new Options { TypeCorrection = TypeCorrectionMode.None });
        var root = (JObject)((JObject)result.Jtoken)["root"];

        Assert.AreEqual("true", root["fax"]?["@xsi:nil"]?.ToString());
        Assert.AreEqual("false", root["phone"]?["@xsi:nil"]?.ToString());
    }

    [TestMethod]
    public void AttributesMode_XsiNilTrue_EmptyElement_ShouldBecomeNull()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                      <fax xsi:nil='true'/>
                    </root>"
        };

        var result = JSON.ConvertXMLStringToJToken(input, new Options { TypeCorrection = TypeCorrectionMode.Attributes });
        var fax = ((JObject)result.Jtoken)["root"]?["fax"];

        Assert.IsNotNull(fax);
        Assert.AreEqual(JTokenType.Null, fax.Type);
    }

    [TestMethod]
    public void AttributesMode_XsiNilTrue_WithContent_ShouldUseContent()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                      <fax xsi:nil='true'>123</fax>
                      <count xsi:type='int' xsi:nil='true'>42</count>
                    </root>"
        };

        // Content wins over the nil flag: the string "123" survives, and the typed element
        // still converts to integer 42 via the normal xsi:type path.
        var result = JSON.ConvertXMLStringToJToken(input, new Options { TypeCorrection = TypeCorrectionMode.Attributes });
        var root = (JObject)((JObject)result.Jtoken)["root"];

        Assert.AreEqual(JTokenType.String, root["fax"].Type);
        Assert.AreEqual("123", root["fax"].ToString());
        Assert.AreEqual(JTokenType.Integer, root["count"].Type);
        Assert.AreEqual(42, root["count"].Value<int>());
    }

    [TestMethod]
    public void AttributesMode_XsiNilFalse_EmptyElement_ShouldUseTypeDefault()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                      <count xsi:type='int' xsi:nil='false'/>
                      <ratio xsi:type='float' xsi:nil='false'/>
                      <inStock xsi:type='boolean' xsi:nil='false'/>
                      <str xsi:type='string' xsi:nil='false'/>
                    </root>"
        };

        var result = JSON.ConvertXMLStringToJToken(input, new Options { TypeCorrection = TypeCorrectionMode.Attributes });
        var root = (JObject)((JObject)result.Jtoken)["root"];

        Assert.AreEqual(JTokenType.Integer, root["count"].Type);
        Assert.AreEqual(0L, root["count"].Value<long>());
        Assert.AreEqual(JTokenType.Float, root["ratio"].Type);
        Assert.AreEqual(0d, root["ratio"].Value<double>());
        Assert.AreEqual(JTokenType.Boolean, root["inStock"].Type);
        Assert.IsFalse(root["inStock"].Value<bool>());
        Assert.AreEqual(JTokenType.String, root["str"].Type);
        Assert.AreEqual(string.Empty,  root["str"].Value<string>());
    }

    [TestMethod]
    public void AttributesMode_XsiNilFalse_WithContent_ShouldStripNilAndKeepContent()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                      <count xsi:type='int' xsi:nil='false'>5</count>
                    </root>"
        };

        var result = JSON.ConvertXMLStringToJToken(input, new Options { TypeCorrection = TypeCorrectionMode.Attributes });
        var count = ((JObject)result.Jtoken)["root"]?["count"];

        Assert.AreEqual(JTokenType.Integer, count.Type);
        Assert.AreEqual(5, count.Value<int>());
    }

    [TestMethod]
    public void AttributesMode_XsiNilFalse_WithoutTypeInfo_ShouldBeNoOp()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                      <fax xsi:nil='false'/>
                    </root>"
        };

        // No xsi:type → we have no type info to coerce against. Per spec, leave the wrapper
        // alone (xsi:nil attribute stays so the caller can see the marker if they want).
        var result = JSON.ConvertXMLStringToJToken(input, new Options { TypeCorrection = TypeCorrectionMode.Attributes });
        var fax = ((JObject)result.Jtoken)["root"]?["fax"];

        Assert.IsInstanceOfType(fax, typeof(JObject));
        Assert.AreEqual("false", fax["@xsi:nil"].ToString());
    }

    [TestMethod]
    public void AttributesMode_XsiNilFalse_PreservesOtherAttributes()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                      <price currency='EUR' xsi:type='float' xsi:nil='false'/>
                    </root>"
        };

        // Element carries another data attribute, so the wrapper must survive. xsi:nil and
        // xsi:type are consumed; #text becomes the typed default; @currency stays untouched.
        var result = JSON.ConvertXMLStringToJToken(input, new Options { TypeCorrection = TypeCorrectionMode.Attributes });
        var price = ((JObject)result.Jtoken)["root"]?["price"];

        Assert.IsInstanceOfType(price, typeof(JObject));
        Assert.AreEqual("EUR", price["@currency"].ToString());
        Assert.IsNull(price["@xsi:type"]);
        Assert.IsNull(price["@xsi:nil"]);
        Assert.AreEqual(JTokenType.Float, price["#text"].Type);
        Assert.AreEqual(0d, price["#text"].Value<double>());
    }

    [TestMethod]
    public void AttributesMode_XsiNilFalse_HonoursAliasedPrefix()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:i='http://www.w3.org/2001/XMLSchema-instance'>
                      <count i:type='int' i:nil='false'/>
                    </root>"
        };

        var result = JSON.ConvertXMLStringToJToken(input, new Options { TypeCorrection = TypeCorrectionMode.Attributes });
        var count = ((JObject)result.Jtoken)["root"]?["count"];

        Assert.AreEqual(JTokenType.Integer, count.Type);
        Assert.AreEqual(0L, count.Value<long>());
    }

    [TestMethod]
    public void SchemaMode_XsiNilFalse_EmptyStringElement_ShouldBecomeEmptyString()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                      <name xsi:nil='false'/>
                    </root>"
        };

        var options = new Options()
        {
            TypeCorrection = TypeCorrectionMode.Schema,
            XSD = @"<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema'>
                      <xs:element name='root'>
                        <xs:complexType>
                          <xs:sequence>
                            <xs:element name='name' type='xs:string' nillable='true' />
                          </xs:sequence>
                        </xs:complexType>
                      </xs:element>
                    </xs:schema>"
        };

        // Schema declares the element as xs:string. xsi:nil='false' on an empty element means
        // "do not allow null" — for a string that's an empty string.
        var result = JSON.ConvertXMLStringToJToken(input, options);
        var name = ((JObject)result.Jtoken)["root"]?["name"];

        Assert.IsNotNull(name);
        Assert.AreEqual(JTokenType.String, name.Type);
        Assert.AreEqual(string.Empty, name.ToString());
    }

    [TestMethod]
    public void SchemaMode_XsiNilTrue_EmptyNillableElement_ShouldBecomeNull()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                      <fax xsi:nil='true'/>
                    </root>"
        };

        var options = new Options()
        {
            TypeCorrection = TypeCorrectionMode.Schema,
            XSD = @"<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema'>
                      <xs:element name='root'>
                        <xs:complexType>
                          <xs:sequence>
                            <xs:element name='fax' type='xs:string' nillable='true' />
                          </xs:sequence>
                        </xs:complexType>
                      </xs:element>
                    </xs:schema>"
        };

        var result = JSON.ConvertXMLStringToJToken(input, options);
        var fax = ((JObject)result.Jtoken)["root"]?["fax"];

        Assert.IsNotNull(fax);
        Assert.AreEqual(JTokenType.Null, fax.Type);
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
        Assert.IsTrue(result.Success);

        var root = ((JObject)result.Jtoken)["root"] as JObject;
        Assert.IsNotNull(root);

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
    public void SchemaMode_ShouldHonourAliasedXsiNamespacePrefix()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:i='http://www.w3.org/2001/XMLSchema-instance'>
                      <price>12.5</price>
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
                          </xs:sequence>
                        </xs:complexType>
                      </xs:element>
                    </xs:schema>"
        };

        // Root already binds the XSD-instance namespace under prefix 'i'. The schema-injected
        // type attribute should serialize as @i:type and the prefix-agnostic lookup must still
        // resolve it for conversion.
        var result = JSON.ConvertXMLStringToJToken(input, options);
        var price = ((JObject)result.Jtoken)["root"]?["price"];

        Assert.IsTrue(result.Success);
        Assert.AreEqual(JTokenType.Float, price.Type);
        Assert.AreEqual(12.5, price.Value<double>());
    }

    [TestMethod]
    public void SchemaMode_ShouldOverrideInlineXsiTypeWithSchemaType()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                      <price xsi:type='int'>12.5</price>
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
                          </xs:sequence>
                        </xs:complexType>
                      </xs:element>
                    </xs:schema>"
        };

        // The author lied with xsi:type='int' on a 12.5 value; schema says float. XSD wins.
        var result = JSON.ConvertXMLStringToJToken(input, options);
        var price = ((JObject)result.Jtoken)["root"]?["price"];

        Assert.IsTrue(result.Success);
        Assert.AreEqual(JTokenType.Float, price.Type);
        Assert.AreEqual(12.5, price.Value<double>());
    }

    [TestMethod]
    public void SchemaMode_ShouldStripInlineXsiTypeWhenSchemaTypeIsString()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                      <name xsi:type='int'>42</name>
                    </root>"
        };

        var options = new Options()
        {
            TypeCorrection = TypeCorrectionMode.Schema,
            XSD = @"<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema'>
                      <xs:element name='root'>
                        <xs:complexType>
                          <xs:sequence>
                            <xs:element name='name' type='xs:string' />
                          </xs:sequence>
                        </xs:complexType>
                      </xs:element>
                    </xs:schema>"
        };

        // Inline xsi:type='int' must not override the schema's xs:string declaration.
        var result = JSON.ConvertXMLStringToJToken(input, options);
        var name = ((JObject)result.Jtoken)["root"]?["name"];

        Assert.IsTrue(result.Success);
        Assert.AreEqual(JTokenType.String, name.Type);
        Assert.AreEqual("42", name.ToString());
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
        Assert.IsTrue(result.Success);

        var price = ((JObject)result.Jtoken)["root"]?["price"];
        Assert.IsNotNull(price);
        Assert.AreEqual(JTokenType.String, price.Type);
        Assert.AreEqual("12.5", price.ToString());
    }

    [TestMethod]
    public void SchemaMode_WithoutXsd_ShouldNotConvertInlineXsiTypes()
    {
        var input = new Input()
        {
            XML = @"<root xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                      <price xsi:type='float'>12.5</price>
                    </root>"
        };

        // Schema-without-XSD is a true no-op: inline xsi:type must not bleed through and
        // start converting values either. Tenants who land here via migration with no XSD
        // should see exactly the pre-2.0.0 output shape.
        var result = JSON.ConvertXMLStringToJToken(input, new Options { TypeCorrection = TypeCorrectionMode.Schema });
        var price = ((JObject)result.Jtoken)["root"]?["price"];

        Assert.IsTrue(result.Success);
        Assert.IsInstanceOfType(price, typeof(JObject));
        Assert.AreEqual("float", price["@xsi:type"]?.ToString());
        Assert.AreEqual(JTokenType.String, price["#text"].Type);
        Assert.AreEqual("12.5", price["#text"].ToString());
    }
}