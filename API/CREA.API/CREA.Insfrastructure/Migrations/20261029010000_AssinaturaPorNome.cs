using CREA.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CREA.Infrastructure.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261029010000_AssinaturaPorNome")]
public partial class AssinaturaPorNome : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "ImagemAssinatura",
            table: "Assinaturas",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("UPDATE Assinaturas SET ImagemAssinatura = '' WHERE ImagemAssinatura IS NULL");
        migrationBuilder.AlterColumn<string>(
            name: "ImagemAssinatura",
            table: "Assinaturas",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);
    }
}
