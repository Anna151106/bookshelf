using Bookshelf.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bookshelf.Web.Infrastructure;

public static class ControllerResultExtensions
{
    public const string SuccessKey = "Success";
    public const string ErrorKey = "Error";

    public static void AddErrors(this ModelStateDictionary modelState, Result result)
    {
        foreach (var (key, messages) in result.ValidationErrors)
        {
            foreach (var message in messages)
            {
                modelState.AddModelError(key, message);
            }
        }
    }

    public static void SetSuccess(this Controller controller, string message) => controller.TempData[SuccessKey] = message;

    public static void SetError(this Controller controller, string message) => controller.TempData[ErrorKey] = message;
}
