using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CREA.Infrastructure.Migrations
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
                    LogAuditoriaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_LogsAuditoria", x => x.LogAuditoriaId);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioId);
                });

            migrationBuilder.CreateTable(
                name: "Assinaturas",
                columns: table => new
                {
                    AssinaturaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoEntidade = table.Column<int>(type: "int", nullable: false),
                    EntidadeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoAssinante = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HashAssinatura = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DataAssinatura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImagemAssinatura = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IpAssinante = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Navegador = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    SistemaOperacional = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Dispositivo = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assinaturas", x => x.AssinaturaId);
                    table.ForeignKey(
                        name: "FK_Assinaturas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Profissionais",
                columns: table => new
                {
                    ProfissionalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Cpf = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    NumeroRegistro = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TipoRegistro = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Empresa = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_Profissionais", x => x.ProfissionalId);
                    table.ForeignKey(
                        name: "FK_Profissionais_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Proprietarios",
                columns: table => new
                {
                    ProprietarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Cpf = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proprietarios", x => x.ProprietarioId);
                    table.ForeignKey(
                        name: "FK_Proprietarios_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Obras",
                columns: table => new
                {
                    ObraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Cep = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    ProprietarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Empresa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroCaderneta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroArt = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NumeroRT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoObra = table.Column<int>(type: "int", nullable: false),
                    TipoEdificacao = table.Column<int>(type: "int", nullable: true),
                    AtividadeTecnica = table.Column<int>(type: "int", nullable: true),
                    DirecaoTecnica = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataPrevisaoTermino = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AreaConstruir = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AreaRegularizar = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AreaAmpliar = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AreaReformar = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AreaTotalEdificada = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ValorRecibo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProfissionalResponsavelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioCriadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obras", x => x.ObraId);
                    table.ForeignKey(
                        name: "FK_Obras_Profissionais_ProfissionalResponsavelId",
                        column: x => x.ProfissionalResponsavelId,
                        principalTable: "Profissionais",
                        principalColumn: "ProfissionalId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Obras_Proprietarios_ProprietarioId",
                        column: x => x.ProprietarioId,
                        principalTable: "Proprietarios",
                        principalColumn: "ProprietarioId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Obras_Usuarios_UsuarioCriadorId",
                        column: x => x.UsuarioCriadorId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RelatosVisita",
                columns: table => new
                {
                    RelatoVisitaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ObraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroSequencial = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Atividades = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EquipePresente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CondicaoClimatica = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observacoes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServicosPreliminar = table.Column<bool>(type: "bit", nullable: false),
                    Fundacao = table.Column<bool>(type: "bit", nullable: false),
                    Alvenarias = table.Column<bool>(type: "bit", nullable: false),
                    Superestrutura = table.Column<bool>(type: "bit", nullable: false),
                    Cobertura = table.Column<bool>(type: "bit", nullable: false),
                    EsquadriasInstalacoesEletricasHidraulicas = table.Column<bool>(type: "bit", nullable: false),
                    RevestimentoForroParePiso = table.Column<bool>(type: "bit", nullable: false),
                    Pintura = table.Column<bool>(type: "bit", nullable: false),
                    ServicosComplementares = table.Column<bool>(type: "bit", nullable: false),
                    PosicaoObra = table.Column<int>(type: "int", nullable: true),
                    DecisoesTecnicas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatosVisita", x => x.RelatoVisitaId);
                    table.ForeignKey(
                        name: "FK_RelatosVisita_Obras_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obras",
                        principalColumn: "ObraId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RelatosVisita_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TermosConclusao",
                columns: table => new
                {
                    TermoConclusaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ObraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroTermo = table.Column<int>(type: "int", nullable: false),
                    DataConclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Empresa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Proprietario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TelefoneProprietario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocalObra = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeclaracaoTexto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocalDeclaracao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataDeclaracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProfissionalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermosConclusao", x => x.TermoConclusaoId);
                    table.ForeignKey(
                        name: "FK_TermosConclusao_Obras_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obras",
                        principalColumn: "ObraId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TermosConclusao_Profissionais_ProfissionalId",
                        column: x => x.ProfissionalId,
                        principalTable: "Profissionais",
                        principalColumn: "ProfissionalId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Anexos",
                columns: table => new
                {
                    AnexoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomeArquivo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    NomeArquivoOriginal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaminhoArquivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoArquivo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                    ObraId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RelatoVisitaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anexos", x => x.AnexoId);
                    table.ForeignKey(
                        name: "FK_Anexos_Obras_ObraId",
                        column: x => x.ObraId,
                        principalTable: "Obras",
                        principalColumn: "ObraId");
                    table.ForeignKey(
                        name: "FK_Anexos_RelatosVisita_RelatoVisitaId",
                        column: x => x.RelatoVisitaId,
                        principalTable: "RelatosVisita",
                        principalColumn: "RelatoVisitaId");
                    table.ForeignKey(
                        name: "FK_Anexos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Anexos_ObraId",
                table: "Anexos",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_Anexos_RelatoVisitaId",
                table: "Anexos",
                column: "RelatoVisitaId");

            migrationBuilder.CreateIndex(
                name: "IX_Anexos_UsuarioId",
                table: "Anexos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Assinaturas_TipoEntidade_EntidadeId_TipoAssinante",
                table: "Assinaturas",
                columns: new[] { "TipoEntidade", "EntidadeId", "TipoAssinante" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assinaturas_UsuarioId",
                table: "Assinaturas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Obras_ProfissionalResponsavelId",
                table: "Obras",
                column: "ProfissionalResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_Obras_ProprietarioId",
                table: "Obras",
                column: "ProprietarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Obras_UsuarioCriadorId",
                table: "Obras",
                column: "UsuarioCriadorId");

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
                name: "IX_Proprietarios_UsuarioId",
                table: "Proprietarios",
                column: "UsuarioId",
                unique: true,
                filter: "[UsuarioId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RelatosVisita_ObraId",
                table: "RelatosVisita",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_RelatosVisita_UsuarioId",
                table: "RelatosVisita",
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
                name: "Assinaturas");

            migrationBuilder.DropTable(
                name: "LogsAuditoria");

            migrationBuilder.DropTable(
                name: "TermosConclusao");

            migrationBuilder.DropTable(
                name: "RelatosVisita");

            migrationBuilder.DropTable(
                name: "Obras");

            migrationBuilder.DropTable(
                name: "Profissionais");

            migrationBuilder.DropTable(
                name: "Proprietarios");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
