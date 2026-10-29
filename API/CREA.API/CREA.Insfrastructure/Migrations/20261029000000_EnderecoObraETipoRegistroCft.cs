using CREA.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CREA.Infrastructure.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261029000000_EnderecoObraETipoRegistroCft")]
public partial class EnderecoObraETipoRegistroCft : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Cpf",
            table: "Usuarios",
            type: "nvarchar(14)",
            maxLength: 14,
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Usuarios_Cpf",
            table: "Usuarios",
            column: "Cpf",
            unique: true,
            filter: "[Cpf] IS NOT NULL AND [Cpf] <> ''");

        migrationBuilder.Sql("UPDATE u SET Cpf = p.Cpf FROM Usuarios AS u INNER JOIN Profissionais AS p ON p.UsuarioId = u.UsuarioId WHERE u.Cpf IS NULL AND (SELECT COUNT(*) FROM Profissionais AS p2 WHERE p2.Cpf = p.Cpf) = 1");

        migrationBuilder.AddColumn<string>(
            name: "Logradouro",
            table: "Obras",
            type: "nvarchar(200)",
            maxLength: 200,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "Quadra",
            table: "Obras",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "Lote",
            table: "Obras",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<int>(
            name: "Numero",
            table: "Obras",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.Sql("UPDATE Obras SET Logradouro = LocalObra WHERE Logradouro = ''");
        migrationBuilder.Sql("UPDATE Profissionais SET TipoRegistro = 'CFT' WHERE UPPER(TipoRegistro) = 'CRT'");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "Logradouro", table: "Obras");
        migrationBuilder.DropColumn(name: "Quadra", table: "Obras");
        migrationBuilder.DropColumn(name: "Lote", table: "Obras");
        migrationBuilder.DropColumn(name: "Numero", table: "Obras");
        migrationBuilder.Sql("UPDATE Profissionais SET TipoRegistro = 'CRT' WHERE UPPER(TipoRegistro) = 'CFT'");
        migrationBuilder.DropIndex(name: "IX_Usuarios_Cpf", table: "Usuarios");
        migrationBuilder.DropColumn(name: "Cpf", table: "Usuarios");
    }
}
