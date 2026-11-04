using Bookshelf.Application.Common;

namespace Bookshelf.Web.Controllers.Api;

public record Select2Response(IReadOnlyList<LookupItemDto> Results);
