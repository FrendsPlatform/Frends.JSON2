using Frends.JSON.QuerySingle.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace Frends.JSON.QuerySingle.UnitTests;

[TestClass]
public class ErrorHandlerTests
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    private static Input InvalidInput() => new Input
    {
        Json = "not valid json",
        Query = "$.key"
    };

    private static Options DefaultOptions() => new Options
    {
        ErrorWhenNotMatched = false,
        ThrowErrorOnFailure = true,
        ErrorMessageOnFailure = string.Empty
    };

    [TestMethod]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var ex = Assert.ThrowsException<Exception>((Action)(() =>
            JSON.QuerySingle(InvalidInput(), DefaultOptions(), CancellationToken.None)));
        Assert.IsNotNull(ex);
    }

    [TestMethod]
    public void Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;
        var result = JSON.QuerySingle(InvalidInput(), options, CancellationToken.None);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = DefaultOptions();
        options.ErrorMessageOnFailure = CustomErrorMessage;
        var ex = Assert.ThrowsException<Exception>((Action)(() =>
            JSON.QuerySingle(InvalidInput(), options, CancellationToken.None)));
        Assert.IsNotNull(ex);
        Assert.IsTrue(ex.Message.Contains(CustomErrorMessage));
    }
}
