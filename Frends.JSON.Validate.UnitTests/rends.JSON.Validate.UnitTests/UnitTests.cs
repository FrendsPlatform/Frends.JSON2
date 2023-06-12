using Frends.JSON.Validate.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.JSON.Validate.UnitTests;

[TestClass]
public class UnitTests
{
    private Input _input = new();
    private Options _options = new();
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

    [TestInitialize]
    public void StartUp()
    {
        _input = new Input() { Json = ValidUserJson, JsonSchema = ValidUserSchema };
        _options = new Options() { ThrowOnInvalidJson = true };
    }

    [TestMethod]
    public void JsonShouldValidate()
    {
        var result = JSON.Validate(_input, _options);
        Assert.IsTrue(result.IsValid);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(0, result.Errors.Count);
    }

    [TestMethod]
    public void ShouldHaveLicenseSetForExecutingMoreThan1000Validations()
    {
        var results = Enumerable.Range(0, 2000).Select(i => JSON.Validate(_input, _options)).ToList();

        foreach (var result in results)
        {
            Assert.IsTrue(result.IsValid);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.Errors.Count);
        }
    }

    [TestMethod]
    public void JsonShouldNotValidate()
    {
        var user = @"{
              'name': 'Arnie Admin',
              'roles': ['Developer', 'Administrator']
            }";

        var schema = @"{
              'type': 'object',
              'properties': {
                'name': {'type':'string'},
                'roles': {'type': 'object'}
              }
            }";

        var input = _input;
        input.Json = user;
        input.JsonSchema = schema;

        var result = JSON.Validate(input, _options);
        Assert.IsFalse(result.IsValid);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.AreEqual("Invalid type. Expected Object but got Array. Path 'roles', line 3, position 24.", result.Errors[0]);
    }
}