using Bookshelf.Application.Abstractions;
using Microsoft.AspNetCore.Hosting;

namespace Bookshelf.Infrastructure.Storage;

public class LocalFileStorage(IWebHostEnvironment environment) : IFileStorage
{
    private const string CoversFolder = "uploads/covers";
    private const long MaxFileSize = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

    private string WebRoot => environment.WebRootPath;

    public async Task<string> SaveAsync(Stream content, string fileName, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(fileName);
        if (!AllowedExtensions.Contains(extension))
        {
            throw new FileStorageException("Обкладинка має бути у форматі JPG, PNG або WEBP.");
        }

        if (content.CanSeek && content.Length > MaxFileSize)
        {
            throw new FileStorageException("Файл завеликий, максимум 5 МБ.");
        }

        var directory = Path.Combine(WebRoot, CoversFolder);
        Directory.CreateDirectory(directory);

        var relativePath = $"{CoversFolder}/{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var fullPath = Path.Combine(WebRoot, relativePath);

        try
        {
            await using var target = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true);
            await CopyWithLimitAsync(content, target, cancellationToken);
        }
        catch
        {
            File.Delete(fullPath);
            throw;
        }

        return relativePath;
    }

    public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        var coversRoot = Path.GetFullPath(Path.Combine(WebRoot, CoversFolder)) + Path.DirectorySeparatorChar;
        var fullPath = Path.GetFullPath(Path.Combine(WebRoot, path.TrimStart('/', '\\')));

        if (!fullPath.StartsWith(coversRoot, StringComparison.Ordinal))
        {
            throw new FileStorageException("Неправильний шлях до файлу.");
        }

        File.Delete(fullPath);
        return Task.CompletedTask;
    }

    private static async Task CopyWithLimitAsync(Stream source, Stream target, CancellationToken cancellationToken)
    {
        var buffer = new byte[81920];
        long total = 0;
        int read;
        while ((read = await source.ReadAsync(buffer, cancellationToken)) > 0)
        {
            total += read;
            if (total > MaxFileSize)
            {
                throw new FileStorageException("Файл завеликий, максимум 5 МБ.");
            }

            await target.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
        }
    }
}
