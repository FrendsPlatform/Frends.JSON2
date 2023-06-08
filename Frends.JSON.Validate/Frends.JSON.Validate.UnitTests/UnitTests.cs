using Frends.JSON.Validate.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Xunit;

namespace Frends.JSON.Validate.UnitTests;

[TestClass]
public class UnitTests
{
    const string ValidUserJson = @"{
              'name': 'Arnie Admin',
              'roles': ['Developer', 'Administrator']
            }";

    const string ValidUserSchema = @"{
              'type': 'object',
              'properties': {
                'name': {'type':'string'},
                'roles': {'type': 'array'}
              }
            }";

    [Fact]
    public void JsonShouldValidate()
    {
        var result = JSON.Validate(new Input() { Json = ValidUserJson, JsonSchema = ValidUserSchema }, new Options(), default);
        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(0, result.Errors.Count);
    }

    [Fact]
    public void ShouldHaveLicenseSetForExecutingMoreThan1000Validations()
    {
        var results = Enumerable.Range(0, 2000).Select(i => JSON.Validate(
            new Input { Json = ValidUserJson, JsonSchema = ValidUserSchema },
            new Options(),
            default)).ToList();

        foreach (var result in results)
            Assert.IsTrue(result.IsValid);
    }

    [Fact]
    public void JsonShouldNotValidate()
    {
        const string user = @"{
              'name': 'Arnie Admin',
              'roles': ['Developer', 'Administrator']
            }";

        const string schema = @"{
              'type': 'object',
              'properties': {
                'name': {'type':'string'},
                'roles': {'type': 'object'}
              }
            }";
        var result = JSON.Validate(new Input() { Json = user, JsonSchema = schema }, new Options(), default);
        Assert.IsFalse(result.IsValid);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.AreEqual("Invalid type. Expected Object but got Array. Path 'roles', line 3, position 24.", result.Errors[0]);
    }

    [Fact]
    public void InvalidJsonShouldNotValidate()
    {
        const string user = @"{
              name: Arnie Admin,
              roles: [Developer, Administrator]
            }";

        const string schema = @"{
              'type': 'object',
              'properties': {
                'name': {'type':'string'},
                'roles': {'type': 'object'}
              }
            }";
        var result = JSON.Validate(new Input() { Json = user, JsonSchema = schema }, new Options(), default);
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.AreEqual("Unexpected character encountered while parsing value: A. Path 'name', line 2, position 20.", result.Errors[0]);
    }

    [Fact]
    public void JsonValidationShouldThrow()
    {
        const string user = @"{
              'name': 'Arnie Admin',
              'roles': ['Developer', 'Administrator']
            }";

        const string schema = @"{
              'type': 'object',
              'properties': {
                'name': {'type':'string'},
                'roles': {'type': 'object'}
              }
            }";
        var ex = Assert.ThrowsException<JsonException>(() => JSON.Validate(new Input() { Json = user, JsonSchema = schema }, new Options() { ThrowOnInvalidJson = true }, default));
        Assert.IsTrue(ex.Message.Contains("Json is not valid. Invalid type. Expected Object but got Array. Path 'roles', line 3, position 24."));

    }

    [Fact]
    public void InvalidJsonShouldThrow()
    {
        const string user = @"{
              name: Arnie Admin,
              roles: [Developer, Administrator]
            }";

        const string schema = @"{
              'type': 'object',
              'properties': {
                'name': {'type':'string'},
                'roles': {'type': 'object'}
              }
            }";
        var ex = Assert.ThrowsException<JsonReaderException>(() => JSON.Validate(new Input() { Json = user, JsonSchema = schema }, new Options() { ThrowOnInvalidJson = true }, default));
        Assert.IsTrue(ex.Message.Contains("Unexpected character encountered while parsing value: A. Path 'name', line 2, position 20."));
    }
}
