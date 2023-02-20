using Frends.JSON.ConvertJSONStringToJToken.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Frends.JSON.ConvertJSONStringToJToken.UnitTests;

[TestClass]
public class UnitTests
{
    [TestMethod]
    public void ShouldConvertJsonStringToJToken()
    {
        var input = new Input()
        {
            Json = @"{ 'foo': 'bar', 'foobar': ['Foo', 'Bar'] }"
        };

        var result = JSON.ConvertJSONStringToJToken(input);
        Assert.IsTrue(result.Success);
        Assert.IsInstanceOfType(result.Jtoken, typeof(JObject));
    }
}