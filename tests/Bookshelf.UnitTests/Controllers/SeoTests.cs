using System.Xml.Linq;
using Bookshelf.Application.Books;
using Bookshelf.Web.Controllers;
using Bookshelf.Web.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Bookshelf.UnitTests.Controllers;

public class SeoTests
{
    [Fact]
    public async Task Sitemap_ContainsAllBooks()
    {
        var books = new Mock<IBookService>();
        books.Setup(s => s.GetAllAsync(null, It.IsAny<CancellationToken>())).ReturnsAsync(
        [
            new BookListDto(1, "1984", 1949, null, 1, "George Orwell", 1, "Dystopia"),
            new BookListDto(2, "Animal Farm", 1945, null, 1, "George Orwell", 1, "Dystopia"),
        ]);
        var controller = new SeoController(books.Object).WithRequest("/sitemap.xml");

        var result = await controller.Sitemap(TestContext.Current.CancellationToken);

        var content = result.Should().BeOfType<ContentResult>().Subject.Content!;
        var locs = XDocument.Parse(content).Descendants().Where(e => e.Name.LocalName == "loc").Select(e => e.Value);
        locs.Should().Equal(
            "http://localhost/",
            "http://localhost/Books",
            "http://localhost/Books/Details/1",
            "http://localhost/Books/Details/2");
    }

    [Fact]
    public void MetaDescription_IsCutTo160Characters()
    {
        var text = new string('а', 300);

        var description = ViewHelpers.MetaDescription(text);

        description.Should().HaveLength(160).And.EndWith("…");
        ViewHelpers.MetaDescription("Короткий опис").Should().Be("Короткий опис");
    }
}
