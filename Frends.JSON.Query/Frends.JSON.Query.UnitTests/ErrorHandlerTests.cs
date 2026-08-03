using Frends.JSON.Query.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace Frends.JSON.Query.UnitTests;

[TestClass]
public class ErrorHandlerTests
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    [TestMethod]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var ex = Assert.ThrowsException<Exception>(() =>
            JSON.Query(DefaultInput(), DefaultOptions(), CancellationToken.None));
        Assert.IsNotNull(ex);
    }

    [TestMethod]
    public void Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;
        var result = JSON.Query(DefaultInput(), options, CancellationToken.None);
        Assert.IsFalse(result.Success);
    }

    [TestMethod]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = DefaultOptions();
        options.ErrorMessageOnFailure = CustomErrorMessage;
        var ex = Assert.ThrowsException<Exception>(() =>
            JSON.Query(DefaultInput(), options, CancellationToken.None));
        Assert.IsNotNull(ex);
        StringAssert.Contains(ex.Message, CustomErrorMessage);
    }

    private static Input DefaultInput() => new()
    {
        Json = "{\"key\":\"value\"}",
        Query = "$.nonexistent"
    };

    private static Options DefaultOptions() =>
        new() { ErrorWhenNotMatched = true, ThrowErrorOnFailure = true, ErrorMessageOnFailure = string.Empty };
}
