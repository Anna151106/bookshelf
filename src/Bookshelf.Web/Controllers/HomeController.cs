using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }

    public IActionResult Status(int id)
    {
        Response.StatusCode = id;
        return View(id);
    }
}
