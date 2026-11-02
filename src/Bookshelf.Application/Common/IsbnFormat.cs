namespace Bookshelf.Application.Common;

public static class IsbnFormat
{
    public static string Normalize(string? value) =>
        string.Concat((value ?? string.Empty).Where(c => c is not ('-' or ' '))).ToUpperInvariant();

    public static bool IsValid(string? value)
    {
        var isbn = Normalize(value);
        return isbn.Length switch
        {
            10 => IsValidIsbn10(isbn),
            13 => IsValidIsbn13(isbn),
            _ => false,
        };
    }

    private static bool IsValidIsbn10(string isbn)
    {
        var sum = 0;
        for (var i = 0; i < 10; i++)
        {
            int digit;
            if (char.IsAsciiDigit(isbn[i]))
            {
                digit = isbn[i] - '0';
            }
            else if (i == 9 && isbn[i] == 'X')
            {
                digit = 10;
            }
            else
            {
                return false;
            }

            sum += digit * (10 - i);
        }

        return sum % 11 == 0;
    }

    private static bool IsValidIsbn13(string isbn)
    {
        if (!isbn.All(char.IsAsciiDigit))
        {
            return false;
        }

        var sum = 0;
        for (var i = 0; i < 13; i++)
        {
            sum += (isbn[i] - '0') * (i % 2 == 0 ? 1 : 3);
        }

        return sum % 10 == 0;
    }
}
