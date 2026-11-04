using System.Xml.Linq;
using Bookshelf.Application.Books;
using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Web.Controllers;

public class SeoController(IBookService bookService) : Controller
{
    [HttpGet("/robots.txt")]
    public IActionResult Robots()
    {
        var text = $"User-agent: *\nAllow: /\n\nSitemap: {SiteUrl()}/sitemap.xml\n";
        return Content(text, "text/plain");
    }

    [HttpGet("/sitemap.xml")]
    public async Task<IActionResult> Sitemap(CancellationToken cancellationToken)
    {
        var books = await bookService.GetAllAsync(cancellationToken: cancellationToken);
        var site = SiteUrl();

        var urls = new List<string> { site + "/", site + "/Books" };
        urls.AddRange(books.Select(b => $"{site}/Books/Details/{b.Id}"));

        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        var xml = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(ns + "urlset", urls.Select(url => new XElement(ns + "url", new XElement(ns + "loc", url)))));

        return Content(xml.Declaration + "\n" + xml.Root, "application/xml");
    }

    private string SiteUrl() => $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
}
