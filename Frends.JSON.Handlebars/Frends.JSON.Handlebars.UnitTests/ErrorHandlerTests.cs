using System;
using System.Threading;
using Frends.JSON.Handlebars.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.JSON.Handlebars.UnitTests;

[TestClass]
public class ErrorHandlerTests
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    private static Input InvalidInput() => new Input
    {
        Json = "not valid json {{{",
        HandlebarTemplate = @"{{title}}",
        HandlebarPartials = new HandlebarPartial[0]
    };

    private static Options DefaultOptions() => new Options { ThrowErrorOnFailure = true };

    [TestMethod]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var ex = Assert.ThrowsException<Exception>(() =>
            JSON.Handlebars(InvalidInput(), DefaultOptions(), CancellationToken.None));
        Assert.IsNotNull(ex);
    }

    [TestMethod]
    public void Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;
        var result = JSON.Handlebars(InvalidInput(), options, CancellationToken.None);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = DefaultOptions();
        options.ErrorMessageOnFailure = CustomErrorMessage;
        var ex = Assert.ThrowsException<Exception>(() =>
            JSON.Handlebars(InvalidInput(), options, CancellationToken.None));
        Assert.IsNotNull(ex);
        StringAssert.Contains(ex.Message, CustomErrorMessage);
    }
}
