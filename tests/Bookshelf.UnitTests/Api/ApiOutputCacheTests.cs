using System.Net.Http.Json;
using System.Text.Json;
using Bookshelf.Application;
using Bookshelf.Application.Abstractions;
using Bookshelf.Infrastructure.Persistence;
using Bookshelf.UnitTests.Infrastructure;
using Bookshelf.Web.Controllers.Api;
using Bookshelf.Web.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Bookshelf.UnitTests.Api;

public sealed class ApiOutputCacheTests : IAsyncLifetime
{
    private readonly TestDatabase database = new();
    private WebApplication app = null!;
    private HttpClient client = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddControllers().AddApplicationPart(typeof(GenresApiController).Assembly);
        builder.Services.AddApplication();
        builder.Services.AddApiOutputCache(builder.Configuration);
        builder.Services.AddScoped<AppDbContext>(sp => database.CreateContext([.. sp.GetServices<ISaveChangesInterceptor>()]));
        builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        builder.Services.AddSingleton(Mock.Of<IFileStorage>());

        app = builder.Build();
        app.UseRouting();
        app.UseOutputCache();
        app.MapControllers();
        await app.StartAsync();
        client = app.GetTestClient();
    }

    public async ValueTask DisposeAsync()
    {
        client.Dispose();
        await app.DisposeAsync();
        database.Dispose();
    }

    [Fact]
    public async Task Get_Repeated_ReturnsCachedResponse()
    {
        database.AddGenre("Drama");
        (await TotalAsync("/api/genres")).Should().Be(1);

        database.AddGenre("Poetry");

        (await TotalAsync("/api/genres")).Should().Be(1);
        (await TotalAsync("/api/genres?limit=5")).Should().Be(2);
    }

    [Fact]
    public async Task Post_Review_InvalidatesReviewsCache()
    {
        var book = database.AddBook(database.AddAuthor(), database.AddGenre());
        (await TotalAsync("/api/reviews")).Should().Be(0);

        var response = await client.PostAsJsonAsync("/api/reviews", new { bookId = book.Id, readerName = "Оксана", rating = 5 }, Ct);
        response.EnsureSuccessStatusCode();

        (await TotalAsync("/api/reviews")).Should().Be(1);
    }

    [Fact]
    public async Task Put_Genre_InvalidatesGenresAndBooksCache()
    {
        var genre = database.AddGenre("Dystopia");
        var book = database.AddBook(database.AddAuthor(), genre);
        (await GetAsync($"/api/genres/{genre.Id}")).GetProperty("name").GetString().Should().Be("Dystopia");
        (await GetAsync($"/api/books/{book.Id}")).GetProperty("genreName").GetString().Should().Be("Dystopia");

        var response = await client.PutAsJsonAsync($"/api/genres/{genre.Id}", new { name = "Антиутопія" }, Ct);
        response.EnsureSuccessStatusCode();

        (await GetAsync($"/api/genres/{genre.Id}")).GetProperty("name").GetString().Should().Be("Антиутопія");
        (await GetAsync($"/api/books/{book.Id}")).GetProperty("genreName").GetString().Should().Be("Антиутопія");
    }

    private async Task<int> TotalAsync(string url) => (await GetAsync(url)).GetProperty("total").GetInt32();

    private async Task<JsonElement> GetAsync(string url) => await client.GetFromJsonAsync<JsonElement>(url, Ct);
}
