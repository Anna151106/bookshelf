using Bookshelf.Application.Libraries;
using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Web.Controllers.Api;

[ApiController]
[Route("api/libraries")]
public class LibrariesApiController(ILibraryService libraryService) : ControllerBase
{
    [HttpGet("map")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<IReadOnlyList<LibraryMapPointDto>> GetMap([FromQuery] LibraryMapFilter filter, CancellationToken cancellationToken) =>
        libraryService.GetForMapAsync(filter, cancellationToken);
}
