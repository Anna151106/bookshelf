using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Web.Controllers.Api;

public record PageQuery
{
    public const int DefaultLimit = 10;
    public const int MaxLimit = 100;

    [FromQuery(Name = "skip")]
    [DefaultValue(0)]
    [Range(0, int.MaxValue, ErrorMessage = "Параметр skip не може бути від'ємним.")]
    public int Skip { get; init; }

    [FromQuery(Name = "limit")]
    [DefaultValue(DefaultLimit)]
    [Range(1, MaxLimit, ErrorMessage = "Параметр limit має бути від 1 до 100.")]
    public int Limit { get; init; } = DefaultLimit;
}
