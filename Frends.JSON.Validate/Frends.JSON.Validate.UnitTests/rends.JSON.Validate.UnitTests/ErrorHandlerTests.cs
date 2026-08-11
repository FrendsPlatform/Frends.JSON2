using Frends.JSON.Validate.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace Frends.JSON.Validate.UnitTests;

[TestClass]
public class ErrorHandlerTests
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    private static Input DefaultInput() => new()
    {
        Json = "not valid json {{{{",
        JsonSchema = @"{'type': 'object'}"
    };

    private static Options DefaultOptions() => new()
    {
        FailOnInvalidJson = true,
        ThrowErrorOnFailure = true,
    };

    [TestMethod]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var ex = Assert.ThrowsException<Exception>(() =>
            JSON.Validate(DefaultInput(), DefaultOptions(), CancellationToken.None));
        Assert.IsNotNull(ex);
    }

    [TestMethod]
    public void Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;
        var result = JSON.Validate(DefaultInput(), options, CancellationToken.None);
        Assert.IsFalse(result.Success);
    }

    [TestMethod]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = DefaultOptions();
        options.ErrorMessageOnFailure = CustomErrorMessage;
        var ex = Assert.ThrowsException<Exception>(() =>
            JSON.Validate(DefaultInput(), options, CancellationToken.None));
        Assert.IsNotNull(ex);
        Assert.IsTrue(ex.Message.Contains(CustomErrorMessage));
    }
}
