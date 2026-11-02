using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bookshelf.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Biography = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    BirthYear = table.Column<int>(type: "integer", nullable: true),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    PublishedYear = table.Column<int>(type: "integer", nullable: false),
                    Isbn = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Pages = table.Column<int>(type: "integer", nullable: false),
                    CoverPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AuthorId = table.Column<int>(type: "integer", nullable: false),
                    GenreId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.CheckConstraint("CK_Books_Pages", "\"Pages\" > 0");
                    table.ForeignKey(
                        name: "FK_Books_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Books_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BookId = table.Column<int>(type: "integer", nullable: false),
                    ReaderName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.CheckConstraint("CK_Reviews_Rating", "\"Rating\" BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_Reviews_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "Biography", "BirthYear", "Country", "FullName" },
                values: new object[,]
                {
                    { 1, "Український письменник, поет, перекладач, учений і громадський діяч.", 1856, "Україна", "Іван Франко" },
                    { 2, "Українська поетеса, драматургиня і перекладачка, авторка драм-феєрій.", 1871, "Україна", "Леся Українка" },
                    { 3, "Сучасний український письменник, поет і музикант.", 1974, "Україна", "Сергій Жадан" },
                    { 4, "Британський письменник і публіцист, автор антиутопій.", 1903, "Велика Британія", "Джордж Орвелл" },
                    { 5, "Американський письменник-фантаст.", 1920, "США", "Рей Бредбері" }
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Роман" },
                    { 2, "Драма" },
                    { 3, "Антиутопія" },
                    { 4, "Наукова фантастика" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "AuthorId", "CoverPath", "Description", "GenreId", "Isbn", "Pages", "PublishedYear", "Title" },
                values: new object[,]
                {
                    { 1, 1, null, "Історична повість про боротьбу громади Тухлі проти монгольської навали.", 1, "9780000000019", 256, 1883, "Захар Беркут" },
                    { 2, 1, null, "Соціально-психологічний роман про адвоката Євгенія Рафаловича.", 1, "9780000000026", 384, 1900, "Перехресні стежки" },
                    { 3, 1, null, "Роман про життя робітників бориславських нафтових промислів.", 1, "9780000000033", 320, 1882, "Борислав сміється" },
                    { 4, 2, null, "Драма-феєрія про кохання Мавки та Лукаша.", 2, "9780000000040", 160, 1911, "Лісова пісня" },
                    { 5, 2, null, "Драма, що по-новому інтерпретує легенду про Дон Жуана.", 2, "9780000000057", 128, 1912, "Камінний господар" },
                    { 6, 3, null, "Роман про повернення Германа на схід України, щоб захистити бензоколонку брата.", 1, "9780000000064", 448, 2010, "Ворошиловград" },
                    { 7, 3, null, "Роман про вчителя, який іде через охоплене війною місто по свого племінника.", 1, "9780000000071", 336, 2017, "Інтернат" },
                    { 8, 3, null, "Цикл історій і віршів про мешканців Харкова.", 1, "9780000000088", 368, 2014, "Месопотамія" },
                    { 9, 4, null, "Антиутопія про тоталітарну державу, де Великий Брат стежить за кожним.", 3, "9780000000095", 328, 1949, "1984" },
                    { 10, 4, null, "Сатирична повість-притча про революцію на фермі.", 3, "9780000000101", 112, 1945, "Колгосп тварин" },
                    { 11, 5, null, "Антиутопія про пожежника, який спалює книжки.", 3, "9780000000118", 256, 1953, "451° за Фаренгейтом" },
                    { 12, 5, null, "Цикл оповідань про колонізацію Марса.", 4, "9780000000125", 288, 1950, "Марсіанські хроніки" }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "BookId", "CreatedAt", "Rating", "ReaderName", "Text" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 1, 10, 12, 0, 0, 0, DateTimeKind.Utc), 5, "Олена", "Сильна історія про єдність громади." },
                    { 2, 1, new DateTime(2025, 1, 18, 12, 0, 0, 0, DateTimeKind.Utc), 4, "Андрій", "Читається легко, хоч мова трохи архаїчна." },
                    { 3, 2, new DateTime(2025, 2, 3, 12, 0, 0, 0, DateTimeKind.Utc), 5, "Марія", "Глибокий психологізм, неочікуваний фінал." },
                    { 4, 3, new DateTime(2025, 2, 14, 12, 0, 0, 0, DateTimeKind.Utc), 4, "Тарас", null },
                    { 5, 4, new DateTime(2025, 2, 20, 12, 0, 0, 0, DateTimeKind.Utc), 5, "Ірина", "Найкрасивіший текст української драматургії." },
                    { 6, 4, new DateTime(2025, 3, 1, 12, 0, 0, 0, DateTimeKind.Utc), 5, "Богдан", "Перечитую щовесни." },
                    { 7, 5, new DateTime(2025, 3, 9, 12, 0, 0, 0, DateTimeKind.Utc), 4, "Наталія", "Цікавий погляд на образ Дон Жуана." },
                    { 8, 6, new DateTime(2025, 3, 15, 12, 0, 0, 0, DateTimeKind.Utc), 5, "Дмитро", "Атмосферно і дуже по-своєму." },
                    { 9, 6, new DateTime(2025, 3, 28, 12, 0, 0, 0, DateTimeKind.Utc), 3, "Софія", "Довгі описи, але фінал вартий того." },
                    { 10, 7, new DateTime(2025, 4, 5, 12, 0, 0, 0, DateTimeKind.Utc), 5, "Олег", "Важка, але необхідна книжка." },
                    { 11, 7, new DateTime(2025, 4, 12, 12, 0, 0, 0, DateTimeKind.Utc), 5, "Юлія", null },
                    { 12, 8, new DateTime(2025, 4, 22, 12, 0, 0, 0, DateTimeKind.Utc), 4, "Максим", "Кожна історія — окремий світ." },
                    { 13, 9, new DateTime(2025, 5, 2, 12, 0, 0, 0, DateTimeKind.Utc), 5, "Анна", "Лякає тим, наскільки актуальна." },
                    { 14, 9, new DateTime(2025, 5, 11, 12, 0, 0, 0, DateTimeKind.Utc), 5, "Віктор", "Класика, яку варто прочитати кожному." },
                    { 15, 9, new DateTime(2025, 5, 19, 12, 0, 0, 0, DateTimeKind.Utc), 4, "Катерина", "Похмуро, але переконливо." },
                    { 16, 10, new DateTime(2025, 6, 1, 12, 0, 0, 0, DateTimeKind.Utc), 5, "Роман", "Коротко і влучно." },
                    { 17, 10, new DateTime(2025, 6, 8, 12, 0, 0, 0, DateTimeKind.Utc), 4, "Оксана", null },
                    { 18, 11, new DateTime(2025, 6, 17, 12, 0, 0, 0, DateTimeKind.Utc), 5, "Павло", "Книжка про цінність книжок." },
                    { 19, 11, new DateTime(2025, 6, 25, 12, 0, 0, 0, DateTimeKind.Utc), 3, "Христина", "Ідея чудова, сюжет простуватий." },
                    { 20, 12, new DateTime(2025, 7, 3, 12, 0, 0, 0, DateTimeKind.Utc), 4, "Ігор", "Поетична фантастика, трохи сумна." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorId",
                table: "Books",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_GenreId",
                table: "Books",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Isbn",
                table: "Books",
                column: "Isbn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Genres_Name",
                table: "Genres",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BookId",
                table: "Reviews",
                column: "BookId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Genres");
        }
    }
}
