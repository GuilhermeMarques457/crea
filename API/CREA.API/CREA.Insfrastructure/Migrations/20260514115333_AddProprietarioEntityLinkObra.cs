using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CREA.Insfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProprietarioEntityLinkObra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Proprietarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Cpf = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proprietarios", x => x.Id);
                });

            migrationBuilder.AddColumn<Guid>(
                name: "ProprietarioId",
                table: "Obras",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql("""
                INSERT INTO [Proprietarios] ([Id], [Nome], [Cpf], [Email], [Telefone], [CriadoEm], [AtualizadoEm], [Ativo])
                SELECT NEWID(), [d].[Nome], N'', N'', CASE WHEN [d].[Tel] = N'' THEN N'' ELSE [d].[Tel] END, GETUTCDATE(), NULL, 1
                FROM (
                    SELECT DISTINCT [Proprietario] AS [Nome], ISNULL([TelefoneProprietario], N'') AS [Tel]
                    FROM [Obras]
                ) AS [d];

                UPDATE [o]
                SET [ProprietarioId] = [p].[Id]
                FROM [Obras] AS [o]
                INNER JOIN [Proprietarios] AS [p]
                    ON [p].[Nome] = [o].[Proprietario]
                    AND ISNULL([p].[Telefone], N'') = ISNULL([o].[TelefoneProprietario], N'');
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "ProprietarioId",
                table: "Obras",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "Proprietario",
                table: "Obras");

            migrationBuilder.DropColumn(
                name: "TelefoneProprietario",
                table: "Obras");

            migrationBuilder.CreateIndex(
                name: "IX_Obras_ProprietarioId",
                table: "Obras",
                column: "ProprietarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Obras_Proprietarios_ProprietarioId",
                table: "Obras",
                column: "ProprietarioId",
                principalTable: "Proprietarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Obras_Proprietarios_ProprietarioId",
                table: "Obras");

            migrationBuilder.DropIndex(
                name: "IX_Obras_ProprietarioId",
                table: "Obras");

            migrationBuilder.AddColumn<string>(
                name: "Proprietario",
                table: "Obras",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TelefoneProprietario",
                table: "Obras",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE [o]
                SET
                    [Proprietario] = [p].[Nome],
                    [TelefoneProprietario] = NULLIF([p].[Telefone], N'')
                FROM [Obras] AS [o]
                INNER JOIN [Proprietarios] AS [p] ON [p].[Id] = [o].[ProprietarioId];
                """);

            migrationBuilder.DropColumn(
                name: "ProprietarioId",
                table: "Obras");

            migrationBuilder.DropTable(
                name: "Proprietarios");
        }
    }
}
