using Bookshelf.Application.Reviews;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Bookshelf.UnitTests.Validators;

public class ReviewValidatorTests
{
    private readonly UpdateReviewDtoValidator validator = new();

    [Theory]
    [InlineData(1, true)]
    [InlineData(5, true)]
    [InlineData(0, false)]
    [InlineData(6, false)]
    public async Task Rating_MustBeFrom1To5(int rating, bool isValid)
    {
        var review = new UpdateReviewDto { Id = 1, ReaderName = "Olena", Rating = rating };

        var result = await validator.TestValidateAsync(review, cancellationToken: TestContext.Current.CancellationToken);

        result.IsValid.Should().Be(isValid);
    }
}
