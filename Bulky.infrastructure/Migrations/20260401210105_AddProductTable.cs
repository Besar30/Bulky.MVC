using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bulky.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListPrice = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Price50 = table.Column<double>(type: "float", nullable: false),
                    Price100 = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "Id", "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[,]
                {
                    { 1, "Christopher Nolan", "A mind-bending thriller about dreams within dreams.", "MOV001", 100.0, 90.0, 80.0, 85.0, "Inception" },
                    { 2, "Christopher Nolan", "A journey through space and time to save humanity.", "MOV002", 110.0, 100.0, 90.0, 95.0, "Interstellar" },
                    { 3, "Christopher Nolan", "Batman faces the Joker in Gotham City.", "MOV003", 95.0, 85.0, 75.0, 80.0, "The Dark Knight" },
                    { 4, "Anthony & Joe Russo", "The Avengers assemble for the final battle.", "MOV004", 120.0, 110.0, 100.0, 105.0, "Avengers: Endgame" },
                    { 5, "James Cameron", "A romantic story set on the ill-fated Titanic ship.", "MOV005", 90.0, 80.0, 70.0, 75.0, "Titanic" },
                    { 6, "The Wachowskis", "A hacker discovers the shocking truth about reality.", "MOV006", 105.0, 95.0, 85.0, 90.0, "The Matrix" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "products");
        }
    }
}
