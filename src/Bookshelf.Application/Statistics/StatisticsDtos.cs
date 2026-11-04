namespace Bookshelf.Application.Statistics;

public record GenreBookCountDto(string Genre, int BookCount);

public record AuthorRatingDto(string Author, double AverageRating);

public record DecadeBookCountDto(int Decade, int BookCount);

public record StatisticsDto(
    IReadOnlyList<GenreBookCountDto> BooksByGenre,
    IReadOnlyList<AuthorRatingDto> AverageRatingByAuthor,
    IReadOnlyList<DecadeBookCountDto> BooksByDecade);
