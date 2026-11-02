using Bookshelf.Domain.Entities;

namespace Bookshelf.Infrastructure.Persistence.Seed;

public static class SeedData
{
    public static readonly Author[] Authors =
    [
        new() { Id = 1, FullName = "Іван Франко", BirthYear = 1856, Country = "Україна",
            Biography = "Український письменник, поет, перекладач, учений і громадський діяч." },
        new() { Id = 2, FullName = "Леся Українка", BirthYear = 1871, Country = "Україна",
            Biography = "Українська поетеса, драматургиня і перекладачка, авторка драм-феєрій." },
        new() { Id = 3, FullName = "Сергій Жадан", BirthYear = 1974, Country = "Україна",
            Biography = "Сучасний український письменник, поет і музикант." },
        new() { Id = 4, FullName = "Джордж Орвелл", BirthYear = 1903, Country = "Велика Британія",
            Biography = "Британський письменник і публіцист, автор антиутопій." },
        new() { Id = 5, FullName = "Рей Бредбері", BirthYear = 1920, Country = "США",
            Biography = "Американський письменник-фантаст." },
    ];

    public static readonly Genre[] Genres =
    [
        new() { Id = 1, Name = "Роман" },
        new() { Id = 2, Name = "Драма" },
        new() { Id = 3, Name = "Антиутопія" },
        new() { Id = 4, Name = "Наукова фантастика" },
    ];

    public static readonly Book[] Books =
    [
        new() { Id = 1, Title = "Захар Беркут", PublishedYear = 1883, Isbn = "9780000000019", Pages = 256, AuthorId = 1, GenreId = 1,
            Description = "Історична повість про боротьбу громади Тухлі проти монгольської навали." },
        new() { Id = 2, Title = "Перехресні стежки", PublishedYear = 1900, Isbn = "9780000000026", Pages = 384, AuthorId = 1, GenreId = 1,
            Description = "Соціально-психологічний роман про адвоката Євгенія Рафаловича." },
        new() { Id = 3, Title = "Борислав сміється", PublishedYear = 1882, Isbn = "9780000000033", Pages = 320, AuthorId = 1, GenreId = 1,
            Description = "Роман про життя робітників бориславських нафтових промислів." },
        new() { Id = 4, Title = "Лісова пісня", PublishedYear = 1911, Isbn = "9780000000040", Pages = 160, AuthorId = 2, GenreId = 2,
            Description = "Драма-феєрія про кохання Мавки та Лукаша." },
        new() { Id = 5, Title = "Камінний господар", PublishedYear = 1912, Isbn = "9780000000057", Pages = 128, AuthorId = 2, GenreId = 2,
            Description = "Драма, що по-новому інтерпретує легенду про Дон Жуана." },
        new() { Id = 6, Title = "Ворошиловград", PublishedYear = 2010, Isbn = "9780000000064", Pages = 448, AuthorId = 3, GenreId = 1,
            Description = "Роман про повернення Германа на схід України, щоб захистити бензоколонку брата." },
        new() { Id = 7, Title = "Інтернат", PublishedYear = 2017, Isbn = "9780000000071", Pages = 336, AuthorId = 3, GenreId = 1,
            Description = "Роман про вчителя, який іде через охоплене війною місто по свого племінника." },
        new() { Id = 8, Title = "Месопотамія", PublishedYear = 2014, Isbn = "9780000000088", Pages = 368, AuthorId = 3, GenreId = 1,
            Description = "Цикл історій і віршів про мешканців Харкова." },
        new() { Id = 9, Title = "1984", PublishedYear = 1949, Isbn = "9780000000095", Pages = 328, AuthorId = 4, GenreId = 3,
            Description = "Антиутопія про тоталітарну державу, де Великий Брат стежить за кожним." },
        new() { Id = 10, Title = "Колгосп тварин", PublishedYear = 1945, Isbn = "9780000000101", Pages = 112, AuthorId = 4, GenreId = 3,
            Description = "Сатирична повість-притча про революцію на фермі." },
        new() { Id = 11, Title = "451° за Фаренгейтом", PublishedYear = 1953, Isbn = "9780000000118", Pages = 256, AuthorId = 5, GenreId = 3,
            Description = "Антиутопія про пожежника, який спалює книжки." },
        new() { Id = 12, Title = "Марсіанські хроніки", PublishedYear = 1950, Isbn = "9780000000125", Pages = 288, AuthorId = 5, GenreId = 4,
            Description = "Цикл оповідань про колонізацію Марса." },
    ];

    public static readonly Review[] Reviews =
    [
        Review(1, 1, "Олена", 5, "Сильна історія про єдність громади.", 2025, 1, 10),
        Review(2, 1, "Андрій", 4, "Читається легко, хоч мова трохи архаїчна.", 2025, 1, 18),
        Review(3, 2, "Марія", 5, "Глибокий психологізм, неочікуваний фінал.", 2025, 2, 3),
        Review(4, 3, "Тарас", 4, null, 2025, 2, 14),
        Review(5, 4, "Ірина", 5, "Найкрасивіший текст української драматургії.", 2025, 2, 20),
        Review(6, 4, "Богдан", 5, "Перечитую щовесни.", 2025, 3, 1),
        Review(7, 5, "Наталія", 4, "Цікавий погляд на образ Дон Жуана.", 2025, 3, 9),
        Review(8, 6, "Дмитро", 5, "Атмосферно і дуже по-своєму.", 2025, 3, 15),
        Review(9, 6, "Софія", 3, "Довгі описи, але фінал вартий того.", 2025, 3, 28),
        Review(10, 7, "Олег", 5, "Важка, але необхідна книжка.", 2025, 4, 5),
        Review(11, 7, "Юлія", 5, null, 2025, 4, 12),
        Review(12, 8, "Максим", 4, "Кожна історія — окремий світ.", 2025, 4, 22),
        Review(13, 9, "Анна", 5, "Лякає тим, наскільки актуальна.", 2025, 5, 2),
        Review(14, 9, "Віктор", 5, "Класика, яку варто прочитати кожному.", 2025, 5, 11),
        Review(15, 9, "Катерина", 4, "Похмуро, але переконливо.", 2025, 5, 19),
        Review(16, 10, "Роман", 5, "Коротко і влучно.", 2025, 6, 1),
        Review(17, 10, "Оксана", 4, null, 2025, 6, 8),
        Review(18, 11, "Павло", 5, "Книжка про цінність книжок.", 2025, 6, 17),
        Review(19, 11, "Христина", 3, "Ідея чудова, сюжет простуватий.", 2025, 6, 25),
        Review(20, 12, "Ігор", 4, "Поетична фантастика, трохи сумна.", 2025, 7, 3),
    ];

    private static Review Review(int id, int bookId, string readerName, int rating, string? text, int year, int month, int day) =>
        new()
        {
            Id = id,
            BookId = bookId,
            ReaderName = readerName,
            Rating = rating,
            Text = text,
            CreatedAt = new DateTime(year, month, day, 12, 0, 0, DateTimeKind.Utc),
        };
}
