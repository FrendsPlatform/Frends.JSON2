using Frends.JSON.Handlebars.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.JSON.Handlebars.UnitTests;

[TestClass]
public class UnitTests
{
    [TestMethod]
    public void HandlebarShouldGenerateTemplate()
    {
        var input = new Input()
        {
            Json = @"{'title':'Mr.', 'name':'Andersson'}",
            HandlebarTemplate = @"<div><span>{{title}}</span> <strong>{{name}}</strong></div>",
            HandlebarPartials = new HandlebarPartial[0]
        };

        var result = JSON.Handlebars(input, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(result.Data.Contains("<span>Mr.</span> <strong>Andersson</strong>"));
    }

    [TestMethod]
    public void HandlebarShouldGeneratePartials()
    {
        var input = new Input()
        {
            Json = @"{'title':'Mr.', 'name':'Andersson'}",
            HandlebarTemplate = @"<div><span>{{title}}</span> {{> strongName}}</div>",
            HandlebarPartials = new[] { new HandlebarPartial { Template = "<strong>{{name}}</strong>", TemplateName = "strongName" } }
        };

        var result = JSON.Handlebars(input, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(result.Data.Contains("<span>Mr.</span> <strong>Andersson</strong>"));
    }
}