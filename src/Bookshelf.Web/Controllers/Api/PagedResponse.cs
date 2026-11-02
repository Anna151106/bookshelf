using Bookshelf.Application.Common;
using Microsoft.AspNetCore.Http.Extensions;

namespace Bookshelf.Web.Controllers.Api;

public record PagedResponse<T>(IReadOnlyList<T> Items, int Total, int Skip, int Limit, string? NextLink);

public static class PagedResponse
{
    public static PagedResponse<T> From<T>(PagedList<T> page, PageQuery query, HttpRequest request)
    {
        var nextSkip = query.Skip + query.Limit;
        var nextLink = nextSkip < page.Total
            ? UriHelper.BuildAbsolute(
                request.Scheme,
                request.Host,
                request.PathBase,
                request.Path,
                QueryString.Create(
                [new KeyValuePair<string, string?>("skip", nextSkip.ToString()), new("limit", query.Limit.ToString())]))
            : null;

        return new PagedResponse<T>(page.Items, page.Total, query.Skip, query.Limit, nextLink);
    }
}
