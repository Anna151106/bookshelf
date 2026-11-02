namespace Bookshelf.Application.Abstractions;

public record FileUpload(Stream Content, string FileName);
