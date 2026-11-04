using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Web.Controllers;

public class StatisticsController : Controller
{
    public IActionResult Index() => View();
}
