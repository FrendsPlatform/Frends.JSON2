using Frends.JSON.QuerySingle.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Frends.JSON.QuerySingle.UnitTests;

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
    public void TestQuerySingle()
    {
        var input = new Input()
        {
            Json = jsonString,
            Query = "$.Manufacturers[?(@.Name == 'Acme Co')]"
        };

        var options = new Options()
        {
            ErrorWhenNotMatched = true,
        };

        var result = JSON.QuerySingle(input, options);
        Assert.IsTrue(result.Success);
        Assert.IsInstanceOfType(result.Data, typeof(JObject));
    }

    [TestMethod]
    public void QueryShouldThrowIfOptionSetAndNothingIsFound()
    {
        var input = new Input()
        {
            Json = jsonString,
            Query = "$.Manufacturer[?(@.Name == 'Acme Co')]"
        };

        var options = new Options()
        {
            ErrorWhenNotMatched = true,
        };

        var ex = Assert.ThrowsException<JsonException>(() => JSON.QuerySingle(input, options));
        Assert.IsTrue(ex.Message.Contains("Property 'Manufacturer' does not exist on JObject."));
    }

    [TestMethod]
    public void QuerySingleShouldNotThrowIfOptionNotSetAndNothingIsFound()
    {
        var input = new Input()
        {
            Json = jsonString,
            Query = "$.Manufacturer[?(@.Name == 'Acme Co')]"
        };

        var options = new Options()
        {
            ErrorWhenNotMatched = false,
        };

        var result = JSON.QuerySingle(input, options);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Data);
    }
}