using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiVille.Migrations
{
    /// <inheritdoc />
    public partial class AddImagestoVillas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImgCopertina",
                table: "Ville",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Immagine5",
                table: "Ville",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Immagine6",
                table: "Ville",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImgCopertina",
                table: "Ville");

            migrationBuilder.DropColumn(
                name: "Immagine5",
                table: "Ville");

            migrationBuilder.DropColumn(
                name: "Immagine6",
                table: "Ville");
        }
    }
}
