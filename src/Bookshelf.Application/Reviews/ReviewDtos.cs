namespace Bookshelf.Application.Reviews;

public record ReviewDto(int Id, int BookId, string BookTitle, string ReaderName, int Rating, string? Text, DateTime CreatedAt);

public interface IReviewInput
{
    string ReaderName { get; }

    int Rating { get; }

    string? Text { get; }
}

public record CreateReviewDto : IReviewInput
{
    public int BookId { get; init; }

    public string ReaderName { get; init; } = string.Empty;

    public int Rating { get; init; }

    public string? Text { get; init; }
}

public record UpdateReviewDto : IReviewInput
{
    public int Id { get; init; }

    public string ReaderName { get; init; } = string.Empty;

    public int Rating { get; init; }

    public string? Text { get; init; }
}
