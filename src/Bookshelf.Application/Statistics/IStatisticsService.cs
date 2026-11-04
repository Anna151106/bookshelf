namespace Bookshelf.Application.Statistics;

public interface IStatisticsService
{
    Task<StatisticsDto> GetAsync(CancellationToken cancellationToken = default);
}
