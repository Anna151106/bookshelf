using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace Bookshelf.UnitTests.Controllers;

internal static class ControllerTestExtensions
{
    public static T WithTempData<T>(this T controller)
        where T : Controller
    {
        controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        return controller;
    }

    public static T WithRequest<T>(this T controller, string path)
        where T : ControllerBase
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost");
        httpContext.Request.Path = path;
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        return controller;
    }

    public static IReadOnlyDictionary<string, string[]> Errors(string key, string message) =>
        new Dictionary<string, string[]> { [key] = [message] };
}
