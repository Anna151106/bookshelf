using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bookshelf.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLibraries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Libraries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    District = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libraries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LibraryBooks",
                columns: table => new
                {
                    LibraryId = table.Column<int>(type: "integer", nullable: false),
                    BookId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibraryBooks", x => new { x.LibraryId, x.BookId });
                    table.ForeignKey(
                        name: "FK_LibraryBooks_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LibraryBooks_Libraries_LibraryId",
                        column: x => x.LibraryId,
                        principalTable: "Libraries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Libraries",
                columns: new[] { "Id", "Address", "District", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { 1, "вул. Велика Васильківська, 12", "Печерський", 50.438000000000002, 30.521000000000001, "Бібліотека №1" },
                    { 2, "вул. Лаврська, 9", "Печерський", 50.433999999999997, 30.556000000000001, "Бібліотека на Печерську" },
                    { 3, "бул. Дружби Народів, 21", "Печерський", 50.417999999999999, 30.539999999999999, "Бібліотека №2" },
                    { 4, "вул. Сагайдачного, 25", "Подільський", 50.463999999999999, 30.518999999999998, "Бібліотека на Подолі" },
                    { 5, "вул. Костянтинівська, 40", "Подільський", 50.469999999999999, 30.506, "Бібліотека №3" },
                    { 6, "просп. Правди, 31", "Подільський", 50.509999999999998, 30.43, "Бібліотека на Виноградарі" },
                    { 7, "вул. Богдана Хмельницького, 28", "Шевченківський", 50.445999999999998, 30.513000000000002, "Бібліотека №4" },
                    { 8, "вул. Мельникова, 18", "Шевченківський", 50.460999999999999, 30.478999999999999, "Бібліотека біля Лук'янівки" },
                    { 9, "вул. Дегтярівська, 33", "Шевченківський", 50.456000000000003, 30.457999999999998, "Бібліотека №5" },
                    { 10, "просп. Оболонський, 16", "Оболонський", 50.505000000000003, 30.498000000000001, "Бібліотека на Оболоні" },
                    { 11, "вул. Героїв Сталінграду, 44", "Оболонський", 50.515999999999998, 30.501000000000001, "Бібліотека №6" },
                    { 12, "просп. Володимира Івасюка, 8", "Оболонський", 50.505000000000003, 30.472999999999999, "Бібліотека на Мінському масиві" },
                    { 13, "вул. Пирогівський шлях, 30", "Голосіївський", 50.380000000000003, 30.5, "Бібліотека №7" },
                    { 14, "просп. Академіка Глушкова, 45", "Голосіївський", 50.368000000000002, 30.459, "Бібліотека на Теремках" },
                    { 15, "просп. Голосіївський, 70", "Голосіївський", 50.395000000000003, 30.515000000000001, "Бібліотека №8" },
                    { 16, "вул. Солом'янська, 7", "Солом'янський", 50.435000000000002, 30.48, "Бібліотека на Солом'янці" },
                    { 17, "просп. Повітрофлотський, 50", "Солом'янський", 50.438000000000002, 30.449999999999999, "Бібліотека №9" },
                    { 18, "просп. Відрадний, 14", "Солом'янський", 50.43, 30.427, "Бібліотека на Відрадному" },
                    { 19, "просп. Перемоги, 98", "Святошинський", 50.456000000000003, 30.387, "Бібліотека на Борщагівці" },
                    { 20, "вул. Якуба Коласа, 11", "Святошинський", 50.439999999999998, 30.370000000000001, "Бібліотека №10" },
                    { 21, "бул. Академіка Вернадського, 63", "Святошинський", 50.465000000000003, 30.355, "Бібліотека на Академмістечку" },
                    { 22, "бул. Верховної Ради, 26", "Дніпровський", 50.451999999999998, 30.605, "Бібліотека на Лівому березі" },
                    { 23, "вул. Райдужна, 33", "Дніпровський", 50.472000000000001, 30.594999999999999, "Бібліотека №11" },
                    { 24, "вул. Березняківська, 12", "Дніпровський", 50.424999999999997, 30.614999999999998, "Бібліотека на Березняках" },
                    { 25, "просп. Петра Григоренка, 22", "Дарницький", 50.395000000000003, 30.638000000000002, "Бібліотека на Позняках" },
                    { 26, "вул. Ревуцького, 9", "Дарницький", 50.402999999999999, 30.655000000000001, "Бібліотека №12" },
                    { 27, "вул. Колекторна, 19", "Дарницький", 50.392000000000003, 30.614999999999998, "Бібліотека на Осокорках" },
                    { 28, "просп. Червоної Калини, 30", "Деснянський", 50.512999999999998, 30.605, "Бібліотека на Троєщині" },
                    { 29, "вул. Лісківська, 4", "Деснянський", 50.497999999999998, 30.629999999999999, "Бібліотека №13" },
                    { 30, "просп. Лісовий, 23", "Деснянський", 50.478000000000002, 30.645, "Бібліотека на Лісовому" }
                });

            migrationBuilder.Sql("""
                INSERT INTO "LibraryBooks" ("LibraryId", "BookId")
                SELECT v."LibraryId", v."BookId"
                FROM (VALUES
                    (1, 1),
                    (1, 2),
                    (1, 4),
                    (1, 6),
                    (1, 7),
                    (1, 10),
                    (2, 1),
                    (2, 2),
                    (2, 8),
                    (2, 9),
                    (2, 11),
                    (3, 1),
                    (3, 6),
                    (4, 1),
                    (4, 4),
                    (4, 10),
                    (5, 1),
                    (5, 6),
                    (5, 7),
                    (5, 8),
                    (5, 9),
                    (5, 10),
                    (5, 11),
                    (6, 1),
                    (6, 4),
                    (6, 5),
                    (6, 7),
                    (6, 9),
                    (7, 1),
                    (7, 5),
                    (7, 11),
                    (8, 5),
                    (8, 8),
                    (8, 10),
                    (8, 11),
                    (9, 3),
                    (9, 4),
                    (9, 9),
                    (10, 2),
                    (10, 3),
                    (10, 7),
                    (10, 11),
                    (10, 12),
                    (11, 2),
                    (11, 3),
                    (11, 4),
                    (11, 7),
                    (11, 8),
                    (11, 10),
                    (11, 12),
                    (12, 2),
                    (12, 3),
                    (12, 8),
                    (12, 9),
                    (12, 12),
                    (13, 2),
                    (13, 3),
                    (13, 4),
                    (13, 11),
                    (13, 12),
                    (14, 3),
                    (14, 5),
                    (14, 6),
                    (14, 10),
                    (14, 12),
                    (15, 3),
                    (15, 5),
                    (15, 7),
                    (15, 8),
                    (15, 9),
                    (15, 12),
                    (16, 3),
                    (16, 4),
                    (16, 5),
                    (16, 6),
                    (16, 7),
                    (16, 9),
                    (16, 11),
                    (16, 12),
                    (17, 10),
                    (17, 12),
                    (18, 4),
                    (18, 6),
                    (18, 8),
                    (18, 10),
                    (18, 12),
                    (19, 9),
                    (19, 11),
                    (19, 12),
                    (20, 6),
                    (20, 7),
                    (20, 12),
                    (21, 2),
                    (21, 4),
                    (21, 7),
                    (21, 8),
                    (21, 10),
                    (21, 12),
                    (22, 2),
                    (22, 6),
                    (22, 9),
                    (22, 11),
                    (23, 2),
                    (23, 4),
                    (23, 5),
                    (24, 2),
                    (24, 5),
                    (24, 6),
                    (24, 8),
                    (24, 10),
                    (25, 4),
                    (25, 5),
                    (25, 7),
                    (25, 8),
                    (25, 9),
                    (25, 11),
                    (26, 6),
                    (26, 7),
                    (26, 9),
                    (27, 10),
                    (27, 11),
                    (28, 4),
                    (28, 6),
                    (28, 8),
                    (28, 10),
                    (28, 11),
                    (29, 7),
                    (29, 9),
                    (30, 3),
                    (30, 4),
                    (30, 6),
                    (30, 7),
                    (30, 10),
                    (30, 11)
                ) AS v("LibraryId", "BookId")
                WHERE EXISTS (SELECT 1 FROM "Books" WHERE "Id" = v."BookId");
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Libraries_District",
                table: "Libraries",
                column: "District");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryBooks_BookId",
                table: "LibraryBooks",
                column: "BookId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LibraryBooks");

            migrationBuilder.DropTable(
                name: "Libraries");
        }
    }
}
