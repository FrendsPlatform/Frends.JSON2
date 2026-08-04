using Frends.JSON.ConvertJSONStringToJToken.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace Frends.JSON.ConvertJSONStringToJToken.UnitTests;

[TestClass]
public class UnitTests
{
    private static Options DefaultOptions() => new Options { ThrowErrorOnFailure = true };

    [TestMethod]
    public void ShouldConvertJsonStringToJToken()
    {
        var input = new Input()
        {
            Json = @"{ 'foo': 'bar', 'foobar': ['Foo', 'Bar'] }"
        };

        var result = JSON.ConvertJSONStringToJToken(input, DefaultOptions(), CancellationToken.None);
        Assert.AreEqual("bar", result.Jtoken.foo.ToString());
        Assert.IsTrue(result.Success);
        Assert.IsInstanceOfType(result.Jtoken, typeof(JObject));
    }
}
