using System;
using System.Threading;
using System.Xml;
using Frends.JSON.ConvertXMLStringToJToken.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.JSON.ConvertXMLStringToJToken.Tests;

[TestClass]
public class ErrorHandlerTests
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    [TestMethod]
    public void ShouldThrowErrorWhenThrowErrorOnFailureIsTrue()
    {
        Assert.ThrowsException<XmlException>(() =>
            JSON.ConvertXMLStringToJToken(InvalidInput(), new Options(), CancellationToken.None));
    }

    [TestMethod]
    public void ShouldReturnFailedResultWhenThrowErrorOnFailureIsFalse()
    {
        var result = JSON.ConvertXMLStringToJToken(
            InvalidInput(),
            new Options { ThrowErrorOnFailure = false },
            CancellationToken.None);

        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public void ShouldUseCustomErrorMessageOnFailure()
    {
        var ex = Assert.ThrowsException<Exception>(() =>
            JSON.ConvertXMLStringToJToken(
                InvalidInput(),
                new Options { ErrorMessageOnFailure = CustomErrorMessage },
                CancellationToken.None));

        StringAssert.Contains(ex.Message, CustomErrorMessage);
    }

    private static Input InvalidInput() => new() { XML = "<root><unclosed></root>" };
}
