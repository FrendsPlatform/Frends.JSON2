using Frends.JSON.QuerySingle.Definitions;
using NUnit.Framework;
using System;
using System.Threading;

namespace Frends.JSON.QuerySingle.UnitTests;

[TestFixture]
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

    [Test]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var ex = Assert.Throws<Exception>((TestDelegate)(() =>
            JSON.QuerySingle(InvalidInput(), DefaultOptions(), CancellationToken.None)));
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;
        var result = JSON.QuerySingle(InvalidInput(), options, CancellationToken.None);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
    }

    [Test]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = DefaultOptions();
        options.ErrorMessageOnFailure = CustomErrorMessage;
        var ex = Assert.Throws<Exception>((TestDelegate)(() =>
            JSON.QuerySingle(InvalidInput(), options, CancellationToken.None)));
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex?.Message, Does.Contain(CustomErrorMessage));
    }
}
