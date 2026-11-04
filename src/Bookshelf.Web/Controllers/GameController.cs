using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Web.Controllers;

public class GameController : Controller
{
    public IActionResult Index() => View();
}
