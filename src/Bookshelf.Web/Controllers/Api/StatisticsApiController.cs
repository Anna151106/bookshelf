using Bookshelf.Application.Statistics;
using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Web.Controllers.Api;

[ApiController]
[Route("api/statistics")]
public class StatisticsApiController(IStatisticsService statisticsService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<StatisticsDto> Get(CancellationToken cancellationToken) =>
        statisticsService.GetAsync(cancellationToken);
}
