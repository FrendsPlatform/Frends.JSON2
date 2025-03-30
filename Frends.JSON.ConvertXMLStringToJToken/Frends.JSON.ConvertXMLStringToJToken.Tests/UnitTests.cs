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
    public void ConvertXMLStringToJToken_WithConvertAsArrayPaths_ShouldTreatSingleItemsAsArrays()
    {
        var input = new Input()
        {
            XML = @"<root>
                   <items>
                     <item>item1</item>
                   </items>
                   <categories>
                     <category>category1</category>
                   </categories>
                </root>",
            ConvertAsArrayPaths = new[] { "$.root.items.item" }
        };
        var result = JSON.ConvertXMLStringToJToken(input);

        Assert.IsTrue(result.Success);

        Assert.AreEqual(result.Jtoken["root"]["items"]["item"].Type, JTokenType.Array);
        Assert.AreEqual(result.Jtoken["root"]["items"]["item"][0].ToString(), "item1");

        Assert.AreEqual(result.Jtoken["root"]["categories"]["category"].Type, JTokenType.String);
        Assert.AreEqual(result.Jtoken["root"]["categories"]["category"].ToString(), "category1");
    }
}