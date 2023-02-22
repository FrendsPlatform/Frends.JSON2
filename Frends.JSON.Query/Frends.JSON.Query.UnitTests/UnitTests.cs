using Frends.JSON.Query.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Frends.JSON.Query.UnitTests;

[TestClass]
public class UnitTests
{
    private const string jsonString = @"{
              'Stores': [
                'Lambton Quay',
                'Willis Street'
              ],
              'Manufacturers': [
                {
                  'Name': 'Acme Co',
                  'Products': [
                    {
                      'Name': 'Anvil',
                      'Price': 50
                    }
                  ]
                },
                {
                  'Name': 'Contoso',
                  'Products': [
                    {
                     'Name': 'Elbow Grease',
                      'Price': 99.95
                    },
                    {
                     'Name': 'Headlight Fluid',
                      'Price': 4
                   }
                  ]
                }
              ]
            }";

    [TestMethod]
    public void QueryShouldWorkWithStringInput()
    {
        var input = new Input()
        {
            Json = jsonString,
            Query = "$..Products[?(@.Price >= 50)].Name"
        };

        var options = new Options()
        {
            ErrorWhenNotMatched = true,
        };

        var result = JSON.Query(input, options);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.Data.Count());
        Assert.AreEqual("Anvil", result.Data.First().ToString());
    }

    [TestMethod]
    public void QueryShouldWorkWithJTokenInput()
    {
        var jtoken = JToken.Parse(jsonString);
        var input = new Input()
        {
            Json = jtoken,
            Query = "$..Products[?(@.Price >= 50)].Name"
        };

        var options = new Options()
        {
            ErrorWhenNotMatched = true,
        };

        var result = JSON.Query(input, options);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.Data.Count());
        Assert.AreEqual("Anvil", result.Data.First().ToString());
    }

    [TestMethod]
    public void QueryShouldNotThrowIfOptionNotSetAndNothingIsFound()
    {
        var input = new Input()
        {
            Json = jsonString,
            Query = "$..Product[?(@.Price >= 50)].Name"
        };

        var options = new Options()
        {
            ErrorWhenNotMatched = false,
        };

        var result = JSON.Query(input, options);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsFalse(result.Data.Any());
    }
}