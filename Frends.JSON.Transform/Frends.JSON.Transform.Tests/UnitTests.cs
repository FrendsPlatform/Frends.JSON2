using NUnit.Framework;
using System;
using System.Threading;
using Newtonsoft.Json.Linq;
using Frends.JSON.Transform.Definitions;

namespace Frends.JSON.Transform.Tests;

[TestFixture]
class TestClass
{
    Input _testInput;
    private const string _testJson =
@"
{
""firstName"": ""Veijo"",
""lastName"": ""Frends"",
""age"": 30,
""retired"": false
}
";
    private const string _testMap =
@"
{
""FullName"": ""#xconcat(#valueof($.firstName), ,#valueof($.lastName))"",
""Age"" : ""#valueof($.age)"",
""StillBreething"": ""#valueof($.retired)""
}
";
    private const string _invalidJson = @"{ foo baar";
    [SetUp]
    public void TestSetup()
    {
        _testInput = new Input
        {
            InputJson = _testJson,
            JsonMap = _testMap
        };
    }

    [Test]
    public void TransformShouldAllowJTokenAsInput()
    {
        _testInput.InputJson = JToken.Parse(_testJson);
        var result = JSON.Transform(_testInput, new Options(), default);
        Assert.AreEqual("{\"FullName\":\"Veijo Frends\",\"Age\":30,\"StillBreething\":false}", result.Transformation);
    }

    [Test]
    public void TransformShouldAllowStringAsInput()
    {
        var result = JSON.Transform(_testInput, new Options(), default);
        Assert.AreEqual("{\"FullName\":\"Veijo Frends\",\"Age\":30,\"StillBreething\":false}", result.Transformation);
    }

    [Test]
    public void TransformMapsStringData()
    {
        var result = JSON.Transform(_testInput, new Options(), default);

        var fullName = result.JToken.FullName;

        Assert.AreEqual("Veijo Frends", fullName.ToString());
    }

    [Test]
    public void TransformMapsNumbersCorrectly()
    {
        var result = JSON.Transform(_testInput, new Options(), default);

        var age = result.JToken.Age;

        Assert.AreEqual(JTokenType.Integer, age.Type);
        Assert.AreEqual(30, (int)age);
    }

    [Test]
    public void TransformationMapsBoolValueCorrectly()
    {
        var result = JSON.Transform(_testInput, new Options(), default);

        var breething = result.JToken.StillBreething;

        Assert.AreEqual(JTokenType.Boolean, breething.Type);
        Assert.AreEqual(false, (bool)breething);
    }

    [Test]
    public void TransformWorksWithArray()
    {
        _testInput.InputJson = @"{ ""array"": [{""key"":""first element""},{""key"":""second element""}]}";
        _testInput.JsonMap = @"{""firstElement"":""#valueof($.array[0].key)""}";

        var result = JSON.Transform(_testInput, new Options(), default);
        var firstElement = result.JToken.firstElement;

        Assert.AreEqual("first element", (string)firstElement);
    }

    [Test]
    public void TransformThrowsWithArrayRootElement()
    {
        _testInput.InputJson = @"[{""key"": ""first element""}, {""key"": ""second element""}]";
        _testInput.JsonMap = @"{""firstElement"": ""#valueof($.[0].key)""}";

        var ex = Assert.Throws<Exception>(() => JSON.Transform(_testInput, new Options(), default));
        Assert.That(ex.Message.Equals("Json transformation failed: Input Json is not valid: Array is not supported as root element."));
    }


    [Test]
    public void InvalidJsonInputThrowsException()
    {
        _testInput.InputJson = _invalidJson;

        Assert.Throws<FormatException>(() => JSON.Transform(_testInput, new Options(), default));
    }

    [Test]
    public void InvalidJsonMapThrowsException()
    {
        _testInput.JsonMap = @"{""age"":""#valuof($.age)"", ""foo}";

        Assert.Throws<Exception>(() => JSON.Transform(_testInput, new Options(), default));
    }

    [Test]
    public void TransformReturnsErrorResultWhenThrowErrorOnFailureFalse()
    {
        _testInput.InputJson = _invalidJson;
        var options = new Options { ThrowErrorOnFailure = false };

        var result = JSON.Transform(_testInput, options, default);

        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsNotNull(result.Error.Message);
        Assert.IsInstanceOf<FormatException>(result.Error.AdditionalInfo);
    }

    [Test]
    public void TransformReturnsErrorWithCustomMessageWhenThrowErrorOnFailureFalse()
    {
        _testInput.InputJson = _invalidJson;
        var options = new Options { ThrowErrorOnFailure = false, ErrorMessageOnFailure = "Custom error" };

        var result = JSON.Transform(_testInput, options, default);

        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.That(result.Error.Message.StartsWith("Custom error"));
    }

    [Test]
    public void TransformThrowsWithCustomErrorMessage()
    {
        _testInput.InputJson = _invalidJson;
        var options = new Options { ThrowErrorOnFailure = true, ErrorMessageOnFailure = "Custom error" };

        var ex = Assert.Throws<Exception>(() => JSON.Transform(_testInput, options, default));
        Assert.AreEqual("Custom error", ex.Message);
    }

    [Test]
    public void TransformThrowsOnCancellation()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        Assert.Throws<OperationCanceledException>(() => JSON.Transform(_testInput, new Options(), cts.Token));
    }
}

