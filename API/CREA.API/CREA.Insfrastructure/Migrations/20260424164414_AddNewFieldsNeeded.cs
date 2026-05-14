using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CREA.Insfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsNeeded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssinaturaProprietario",
                table: "TermosConclusao",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAssinaturaProprietario",
                table: "TermosConclusao",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataDeclaracao",
                table: "TermosConclusao",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeclaracaoTexto",
                table: "TermosConclusao",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Empresa",
                table: "TermosConclusao",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocalDeclaracao",
                table: "TermosConclusao",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocalObra",
                table: "TermosConclusao",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumeroTermo",
                table: "TermosConclusao",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Proprietario",
                table: "TermosConclusao",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefoneProprietario",
                table: "TermosConclusao",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Alvenarias",
                table: "RegistrosDiarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Cobertura",
                table: "RegistrosDiarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DecisoesTecnicas",
                table: "RegistrosDiarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsquadriasInstalacoesEletricasHidraulicas",
                table: "RegistrosDiarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Fundacao",
                table: "RegistrosDiarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumeroSequencial",
                table: "RegistrosDiarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Pintura",
                table: "RegistrosDiarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PosicaoObra",
                table: "RegistrosDiarios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RevestimentoForroParePiso",
                table: "RegistrosDiarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ServicosComplementares",
                table: "RegistrosDiarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ServicosPreliminar",
                table: "RegistrosDiarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Superestrutura",
                table: "RegistrosDiarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Empresa",
                table: "Profissionais",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AreaAmpliar",
                table: "Obras",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AreaConstruir",
                table: "Obras",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AreaReformar",
                table: "Obras",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AreaRegularizar",
                table: "Obras",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AreaTotalEdificada",
                table: "Obras",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AtividadeTecnica",
                table: "Obras",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DirecaoTecnica",
                table: "Obras",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Empresa",
                table: "Obras",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroCaderneta",
                table: "Obras",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroRT",
                table: "Obras",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefoneProprietario",
                table: "Obras",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoEdificacao",
                table: "Obras",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorRecibo",
                table: "Obras",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssinaturaProprietario",
                table: "TermosConclusao");

            migrationBuilder.DropColumn(
                name: "DataAssinaturaProprietario",
                table: "TermosConclusao");

            migrationBuilder.DropColumn(
                name: "DataDeclaracao",
                table: "TermosConclusao");

            migrationBuilder.DropColumn(
                name: "DeclaracaoTexto",
                table: "TermosConclusao");

            migrationBuilder.DropColumn(
                name: "Empresa",
                table: "TermosConclusao");

            migrationBuilder.DropColumn(
                name: "LocalDeclaracao",
                table: "TermosConclusao");

            migrationBuilder.DropColumn(
                name: "LocalObra",
                table: "TermosConclusao");

            migrationBuilder.DropColumn(
                name: "NumeroTermo",
                table: "TermosConclusao");

            migrationBuilder.DropColumn(
                name: "Proprietario",
                table: "TermosConclusao");

            migrationBuilder.DropColumn(
                name: "TelefoneProprietario",
                table: "TermosConclusao");

            migrationBuilder.DropColumn(
                name: "Alvenarias",
                table: "RegistrosDiarios");

            migrationBuilder.DropColumn(
                name: "Cobertura",
                table: "RegistrosDiarios");

            migrationBuilder.DropColumn(
                name: "DecisoesTecnicas",
                table: "RegistrosDiarios");

            migrationBuilder.DropColumn(
                name: "EsquadriasInstalacoesEletricasHidraulicas",
                table: "RegistrosDiarios");

            migrationBuilder.DropColumn(
                name: "Fundacao",
                table: "RegistrosDiarios");

            migrationBuilder.DropColumn(
                name: "NumeroSequencial",
                table: "RegistrosDiarios");

            migrationBuilder.DropColumn(
                name: "Pintura",
                table: "RegistrosDiarios");

            migrationBuilder.DropColumn(
                name: "PosicaoObra",
                table: "RegistrosDiarios");

            migrationBuilder.DropColumn(
                name: "RevestimentoForroParePiso",
                table: "RegistrosDiarios");

            migrationBuilder.DropColumn(
                name: "ServicosComplementares",
                table: "RegistrosDiarios");

            migrationBuilder.DropColumn(
                name: "ServicosPreliminar",
                table: "RegistrosDiarios");

            migrationBuilder.DropColumn(
                name: "Superestrutura",
                table: "RegistrosDiarios");

            migrationBuilder.DropColumn(
                name: "Empresa",
                table: "Profissionais");

            migrationBuilder.DropColumn(
                name: "AreaAmpliar",
                table: "Obras");

            migrationBuilder.DropColumn(
                name: "AreaConstruir",
                table: "Obras");

            migrationBuilder.DropColumn(
                name: "AreaReformar",
                table: "Obras");

            migrationBuilder.DropColumn(
                name: "AreaRegularizar",
                table: "Obras");

            migrationBuilder.DropColumn(
                name: "AreaTotalEdificada",
                table: "Obras");

            migrationBuilder.DropColumn(
                name: "AtividadeTecnica",
                table: "Obras");

            migrationBuilder.DropColumn(
                name: "DirecaoTecnica",
                table: "Obras");

            migrationBuilder.DropColumn(
                name: "Empresa",
                table: "Obras");

            migrationBuilder.DropColumn(
                name: "NumeroCaderneta",
                table: "Obras");

            migrationBuilder.DropColumn(
                name: "NumeroRT",
                table: "Obras");

            migrationBuilder.DropColumn(
                name: "TelefoneProprietario",
                table: "Obras");

            migrationBuilder.DropColumn(
                name: "TipoEdificacao",
                table: "Obras");

            migrationBuilder.DropColumn(
                name: "ValorRecibo",
                table: "Obras");
        }
    }
}
