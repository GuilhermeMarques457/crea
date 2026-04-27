using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CREA.Insfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMaisCamposAssinatura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataAssinaturaResponsavel",
                table: "RegistrosDiarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagemAssinaturaResponsavel",
                table: "RegistrosDiarios",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataAssinaturaResponsavel",
                table: "RegistrosDiarios");

            migrationBuilder.DropColumn(
                name: "ImagemAssinaturaResponsavel",
                table: "RegistrosDiarios");
        }
    }
}
