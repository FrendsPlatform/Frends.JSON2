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

        var result = JSON.ConvertXMLStringToJToken(input);
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
             </root>",
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

        var result = JSON.ConvertXMLStringToJToken(input);
        var root = ((JObject)result.Jtoken)["root"];

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(root);
        Assert.IsInstanceOfType(root["person"], typeof(JArray));

        var persons = root["person"] as JArray;

        Assert.IsNotNull(persons);

        Assert.AreEqual(1, persons.Count);
        Assert.AreEqual("Alan", persons[0]["name"]?.ToString());
    }
}
