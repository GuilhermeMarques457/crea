using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CREA.Insfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssinaturaCampo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagemAssinatura",
                table: "AssinaturasTermoConclusao",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IpAssinante",
                table: "AssinaturasTermoConclusao",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagemAssinatura",
                table: "AssinaturasTermoConclusao");

            migrationBuilder.DropColumn(
                name: "IpAssinante",
                table: "AssinaturasTermoConclusao");
        }
    }
}
