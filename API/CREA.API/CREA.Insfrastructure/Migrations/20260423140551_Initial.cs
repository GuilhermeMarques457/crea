using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CREA.Insfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogsAuditoria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NomeUsuario = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Acao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Entidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntidadeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DadosAntigos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DadosNovos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnderecoIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataAcao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogsAuditoria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SenhaHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoUsuario = table.Column<int>(type: "int", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profissionais",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Cpf = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    NumeroRegistro = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TipoRegistro = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Especialidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profissionais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profissionais_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Obras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Cep = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Proprietario = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NumeroArt = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TipoObra = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataPrevisaoTermino = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfissionalResponsavelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioCriadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Obras_Profissionais_ProfissionalResponsavelId",
                        column: x => x.ProfissionalResponsavelId,
                        principalTable: "Profissionais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Obras_Usuarios_UsuarioCriadorId",
                        column: x => x.UsuarioCriadorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ocorrencias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ObraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataOcorrencia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Providencias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ocorrencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ocorrencias_Obras_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ocorrencias_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosDiarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ObraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Atividades = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EquipePresente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CondicaoClimatica = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observacoes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosDiarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosDiarios_Obras_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistrosDiarios_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TermosConclusao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ObraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataConclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfissionalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HashAssinatura = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataAssinatura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermosConclusao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TermosConclusao_Obras_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TermosConclusao_Profissionais_ProfissionalId",
                        column: x => x.ProfissionalId,
                        principalTable: "Profissionais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Anexos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomeArquivo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    NomeArquivoOriginal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaminhoArquivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoArquivo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                    ObraId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RegistroDiarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OcorrenciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anexos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Anexos_Obras_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obras",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Anexos_Ocorrencias_OcorrenciaId",
                        column: x => x.OcorrenciaId,
                        principalTable: "Ocorrencias",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Anexos_RegistrosDiarios_RegistroDiarioId",
                        column: x => x.RegistroDiarioId,
                        principalTable: "RegistrosDiarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Anexos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssinaturasDigitais",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegistroDiarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfissionalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataAssinatura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HashAssinatura = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssinaturasDigitais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssinaturasDigitais_Profissionais_ProfissionalId",
                        column: x => x.ProfissionalId,
                        principalTable: "Profissionais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssinaturasDigitais_RegistrosDiarios_RegistroDiarioId",
                        column: x => x.RegistroDiarioId,
                        principalTable: "RegistrosDiarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Anexos_ObraId",
                table: "Anexos",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_Anexos_OcorrenciaId",
                table: "Anexos",
                column: "OcorrenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Anexos_RegistroDiarioId",
                table: "Anexos",
                column: "RegistroDiarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Anexos_UsuarioId",
                table: "Anexos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_AssinaturasDigitais_ProfissionalId",
                table: "AssinaturasDigitais",
                column: "ProfissionalId");

            migrationBuilder.CreateIndex(
                name: "IX_AssinaturasDigitais_RegistroDiarioId",
                table: "AssinaturasDigitais",
                column: "RegistroDiarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Obras_ProfissionalResponsavelId",
                table: "Obras",
                column: "ProfissionalResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_Obras_UsuarioCriadorId",
                table: "Obras",
                column: "UsuarioCriadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Ocorrencias_ObraId",
                table: "Ocorrencias",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_Ocorrencias_UsuarioId",
                table: "Ocorrencias",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Profissionais_NumeroRegistro",
                table: "Profissionais",
                column: "NumeroRegistro",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profissionais_UsuarioId",
                table: "Profissionais",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosDiarios_ObraId",
                table: "RegistrosDiarios",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosDiarios_UsuarioId",
                table: "RegistrosDiarios",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TermosConclusao_ObraId",
                table: "TermosConclusao",
                column: "ObraId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TermosConclusao_ProfissionalId",
                table: "TermosConclusao",
                column: "ProfissionalId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Anexos");

            migrationBuilder.DropTable(
                name: "AssinaturasDigitais");

            migrationBuilder.DropTable(
                name: "LogsAuditoria");

            migrationBuilder.DropTable(
                name: "TermosConclusao");

            migrationBuilder.DropTable(
                name: "Ocorrencias");

            migrationBuilder.DropTable(
                name: "RegistrosDiarios");

            migrationBuilder.DropTable(
                name: "Obras");

            migrationBuilder.DropTable(
                name: "Profissionais");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
