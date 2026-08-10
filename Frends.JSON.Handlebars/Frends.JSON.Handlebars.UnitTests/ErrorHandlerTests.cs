using System;
using System.Threading;
using Frends.JSON.Handlebars.Definitions;
using NUnit.Framework;

namespace Frends.JSON.Handlebars.UnitTests;

[TestFixture]
internal class ErrorHandlerTests
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    private static Input InvalidInput() => new Input
    {
        Json = "not valid json {{{",
        HandlebarTemplate = @"{{title}}",
        HandlebarPartials = new HandlebarPartial[0]
    };

    private static Options DefaultOptions() => new Options { ThrowErrorOnFailure = true };

    [Test]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var ex = Assert.Throws<Exception>(
            (Action)(() => JSON.Handlebars(InvalidInput(), DefaultOptions(), CancellationToken.None)));
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;
        var result = JSON.Handlebars(InvalidInput(), options, CancellationToken.None);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
    }

    [Test]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = DefaultOptions();
        options.ErrorMessageOnFailure = CustomErrorMessage;
        var ex = Assert.Throws<Exception>(
            (Action)(() => JSON.Handlebars(InvalidInput(), options, CancellationToken.None)));
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex!.Message, Does.Contain(CustomErrorMessage));
    }
}
