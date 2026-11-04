using Bookshelf.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Web.Infrastructure;

public static class ViewHelpers
{
    private const string NoCoverPath = "~/images/no-cover.svg";

    public static string CoverUrl(this IUrlHelper url, string? coverPath) =>
        url.Content(coverPath is null ? NoCoverPath : "~/" + coverPath);

    public static string MetaDescription(string text, int maxLength = 160)
    {
        var clean = string.Join(" ", text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        return clean.Length <= maxLength ? clean : clean[..(maxLength - 1)].TrimEnd() + "…";
    }

    public static string Stars(int rating) =>
        new string('★', rating) + new string('☆', Review.MaxRating - rating);
}
