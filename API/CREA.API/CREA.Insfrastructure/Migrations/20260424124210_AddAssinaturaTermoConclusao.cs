using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CREA.Insfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssinaturaTermoConclusao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AssinadoPeloAdmin",
                table: "TermosConclusao",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AssinadoPeloResponsavel",
                table: "TermosConclusao",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AssinaturasTermoConclusao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TermoConclusaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoAssinante = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HashAssinatura = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataAssinatura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssinaturasTermoConclusao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssinaturasTermoConclusao_TermosConclusao_TermoConclusaoId",
                        column: x => x.TermoConclusaoId,
                        principalTable: "TermosConclusao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssinaturasTermoConclusao_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssinaturasTermoConclusao_TermoConclusaoId",
                table: "AssinaturasTermoConclusao",
                column: "TermoConclusaoId");

            migrationBuilder.CreateIndex(
                name: "IX_AssinaturasTermoConclusao_UsuarioId",
                table: "AssinaturasTermoConclusao",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssinaturasTermoConclusao");

            migrationBuilder.DropColumn(
                name: "AssinadoPeloAdmin",
                table: "TermosConclusao");

            migrationBuilder.DropColumn(
                name: "AssinadoPeloResponsavel",
                table: "TermosConclusao");
        }
    }
}
