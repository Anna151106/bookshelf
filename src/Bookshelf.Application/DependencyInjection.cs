using Bookshelf.Application.Authors;
using Bookshelf.Application.Books;
using Bookshelf.Application.Genres;
using Bookshelf.Application.Reviews;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bookshelf.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, ServiceLifetime.Scoped);
        services.TryAddSingleton(TimeProvider.System);

        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IReviewService, ReviewService>();

        return services;
    }
}
