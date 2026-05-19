# SistemaCREA — Caderneta de Obras Digital

Sistema web desenvolvido para **digitalizar a caderneta de obras** exigida pelo CREA (Conselho Regional de Engenharia e Agronomia), permitindo que profissionais responsáveis técnicos registrem visitas, gerenciem obras, anexem documentos, coletem assinaturas digitais e gerem relatórios em PDF — tudo em conformidade com as obrigações regulatórias.

---

## Sumário

- [Visão Geral](#visão-geral)
- [Tecnologias](#tecnologias)
- [Arquitetura](#arquitetura)
- [Funcionalidades](#funcionalidades)
- [Pré-requisitos](#pré-requisitos)
- [Configuração e Execução](#configuração-e-execução)
  - [Backend](#backend)
  - [Frontend](#frontend)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Endpoints da API](#endpoints-da-api)
- [Papéis de Usuário](#papéis-de-usuário)
- [Seed de Dados](#seed-de-dados)
- [Variáveis de Ambiente](#variáveis-de-ambiente)

---

## Visão Geral

A caderneta de obras é um documento obrigatório para profissionais registrados no CREA. Tradicionalmente mantida em papel, este sistema a substitui por uma plataforma digital com:

- Registro de visitas e atividades diárias na obra
- Controle de etapas construtivas por visita
- Assinaturas digitais com rastreabilidade (IP, dispositivo, hash)
- Geração automática de relatório completo da obra em **PDF**
- Termo de conclusão de obra digital
- Upload de anexos (fotos, plantas, documentos)
- Trilha de auditoria completa de todas as ações do sistema

---

## Tecnologias

### Backend

| Tecnologia            | Versão   | Finalidade                     |
| --------------------- | -------- | ------------------------------ |
| .NET / ASP.NET Core   | 10       | Framework principal da API     |
| Entity Framework Core | 10.0.7   | ORM e migrations               |
| SQL Server            | —        | Banco de dados relacional      |
| JWT Bearer            | 10.0.6   | Autenticação e autorização     |
| QuestPDF              | 2026.5.0 | Geração de relatórios em PDF   |
| Swashbuckle / Swagger | 10.1.7   | Documentação interativa da API |

### Frontend

| Tecnologia       | Versão | Finalidade                      |
| ---------------- | ------ | ------------------------------- |
| Angular          | 21     | Framework SPA                   |
| Angular Material | 21     | Componentes de UI               |
| Tailwind CSS     | 4      | Estilização utilitária          |
| signature_pad    | 5      | Captura de assinaturas digitais |
| Moment.js        | 2.30   | Manipulação de datas            |
| RxJS             | 7.8    | Programação reativa             |

---

## Arquitetura

O projeto segue a **Arquitetura Limpa (Clean Architecture)** no backend, separando responsabilidades em camadas independentes:

```
SistemaCREA/
├── API/
│   └── CREA.API/
│       ├── CREA.Domain/          # Entidades, enums, regras de negócio
│       ├── CREA.Application/     # DTOs, interfaces de repositório, helpers
│       ├── CREA.Infrastructure/  # EF Core, repositórios, migrations, seed
│       └── CREA.API/             # Controllers, Program.cs, serviços HTTP
└── UI/
    └── CREA.CLient/              # Angular SPA (standalone components)
```

O frontend utiliza **Angular Standalone Components** com lazy loading por rota, organizado em `features/`, `core/` (serviços, guards, interceptors) e `shared/` (componentes reutilizáveis).

---

## Funcionalidades

### Gestão de Obras

- Cadastro completo de obra: nome, endereço, número ART/RT, número da caderneta, tipo de obra, tipo de edificação, atividade técnica, áreas (m²), valor do recibo e datas
- Filtros por status (Em Andamento, Concluída, Suspensa, Cancelada), profissional responsável e criador
- Detalhamento completo da obra com todos os seus registros, anexos e assinaturas

### Registros Diários de Visita

- Registro sequencial e numerado de visitas à obra
- Informações por visita: data, atividades realizadas, equipe presente, condição climática, observações, decisões técnicas e posição da obra
- Marcação de etapas construtivas por visita: Serviços Preliminares, Fundação, Alvenarias, Superestrutura, Cobertura, Esquadrias/Instalações, Revestimento, Pintura, Serviços Complementares

### Assinaturas Digitais

- Captura de assinatura via canvas (signature_pad)
- Hash de integridade da assinatura
- Rastreamento completo: IP do assinante, User-Agent, navegador, sistema operacional e tipo de dispositivo
- Tipos de assinante: Responsável Técnico, Proprietário, Fiscal do CREA
- Entidades assinadas: Obras, Registros Diários, Termos de Conclusão
- Lista de assinaturas pendentes por usuário logado

### Termo de Conclusão de Obra

- Emissão de termo digital ao concluir a obra
- Campos: número do termo, data de conclusão, descrição dos serviços, declaração, proprietário e profissional responsável

### Anexos

- Upload de arquivos (fotos, plantas, documentos) vinculados à obra ou a registros diários
- Servidos via endpoint estático `/uploads`

### Relatórios

- Relatório completo da obra consolidando todas as visitas, anexos, assinaturas e termo de conclusão
- **Exportação em PDF** com o nome do arquivo incluindo o nome da obra e a data de geração

### Gestão de Profissionais

- Cadastro de profissionais com CPF, número de registro (CREA, CAU ou CRT), especialidade, empresa, e-mail e telefone
- Vínculo opcional com um usuário do sistema

### Gestão de Proprietários

- Cadastro de proprietários com CPF, e-mail e telefone
- Vínculo opcional com um usuário do sistema

### Gestão de Usuários

- CRUD de usuários do sistema
- Tipos de usuário: Administrador, Responsável Técnico, Usuário CREA, Proprietário

### Pendências

- Painel centralizado com todas as assinaturas pendentes do usuário logado

### Auditoria (Administrador)

- Log completo de todas as ações: entidade afetada, ação, dados antes/depois, usuário, IP e data
- Filtragem por entidade, ação, usuário e período
- Paginação de resultados

---

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) ou SQL Server LocalDB (incluído no Visual Studio)
- [Node.js 20+](https://nodejs.org/) e npm 10+
- [Angular CLI 21](https://angular.dev/tools/cli): `npm install -g @angular/cli`

---

## Configuração e Execução

### Backend

**1. Configure a string de conexão e o JWT**

Edite o arquivo `API/CREA.API/CREA.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=CREA;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "SUA_CHAVE_SECRETA_COM_NO_MINIMO_32_CARACTERES",
    "Issuer": "CREA.API",
    "Audience": "CREA.Client",
    "ExpiracaoHoras": 8
  }
}
```

> **Importante:** A chave JWT deve ter no mínimo 32 caracteres. Nunca commite chaves reais no repositório — use variáveis de ambiente ou `appsettings.Development.json` (ignorado pelo `.gitignore`).

**2. Aplique as migrations e inicie a API**

```bash
cd API/CREA.API/CREA.API
dotnet run
```

Na primeira execução em ambiente de desenvolvimento, o sistema executa automaticamente as migrations e um **seed de dados** com usuários, profissionais, proprietários e obras de exemplo.

A API ficará disponível em:

- `https://localhost:7xxx` ou `http://localhost:5xxx`
- Swagger UI: `https://localhost:7xxx/swagger`

**3. Aplicar migrations manualmente (opcional)**

```bash
cd API/CREA.API
dotnet ef database update --project CREA.Insfrastructure --startup-project CREA.API
```

---

### Frontend

**1. Instale as dependências**

```bash
cd UI/CREA.CLient
npm install
```

**2. Configure a URL da API**

Edite `src/environments/environment.ts` (e `environment.development.ts`) com a URL da API:

```typescript
export const environment = {
  production: false,
  apiUrl: "https://localhost:7xxx/api",
};
```

**3. Inicie o servidor de desenvolvimento**

```bash
npm start
```

A aplicação estará disponível em `http://localhost:4200`.

**4. Build de produção**

```bash
npm run build
```

Os arquivos compilados serão gerados em `dist/`.

---

## Estrutura do Projeto

```
API/CREA.API/
├── CREA.Domain/
│   ├── Entities/
│   │   ├── Obra.cs                 # Entidade principal da caderneta
│   │   ├── RelatoVisita.cs         # Registro diário de visita
│   │   ├── TermoConclusao.cs       # Termo de conclusão de obra
│   │   ├── Profissional.cs         # Responsável técnico
│   │   ├── Proprietario.cs         # Proprietário da obra
│   │   ├── Usuario.cs              # Usuário do sistema
│   │   ├── Anexo.cs                # Arquivo anexo
│   │   ├── Assinatura.cs           # Assinatura digital rastreável
│   │   └── LogAuditoria.cs         # Trilha de auditoria
│   └── Enums/
│       ├── StatusObra.cs
│       ├── TipoObra.cs
│       ├── TipoEdificacao.cs
│       ├── AtividadeTecnica.cs
│       ├── PosicaoObra.cs
│       ├── TipoUsuario.cs
│       ├── TipoAssinante.cs
│       └── TipoEntidadeAssinatura.cs
├── CREA.Application/
│   ├── DTOs/                       # Objetos de transferência por feature
│   ├── Interfaces/Repositories/    # Contratos dos repositórios
│   └── Helpers/                    # Hash de senha, hash de assinatura
├── CREA.Infrastructure/
│   ├── ApplicationDbContext.cs
│   ├── DatabaseSeeder.cs
│   ├── DependencyInjection.cs
│   ├── Migrations/
│   └── Repositories/
└── CREA.API/
    ├── Controllers/
    ├── Services/RelatorioObraPdfComposer.cs
    ├── Helpers/UserAgentInfo.cs
    └── uploads/                    # Arquivos enviados pelos usuários

UI/CREA.CLient/src/app/
├── core/
│   ├── guards/auth.guard.ts        # Protege rotas autenticadas
│   ├── interceptors/auth.interceptor.ts  # Injeta JWT em todas as requisições
│   └── services/                   # Serviços HTTP por domínio
├── features/
│   ├── auth/                       # Login
│   ├── dashboard/                  # Painel inicial
│   ├── obras/                      # Lista, detalhe e formulário de obras
│   ├── registros-diarios/          # Formulário de registro de visita
│   ├── termos-conclusao/           # Formulário do termo
│   ├── profissionais/              # CRUD de profissionais
│   ├── proprietarios/              # CRUD de proprietários
│   ├── usuarios/                   # CRUD de usuários
│   ├── pendencias/                 # Painel de assinaturas pendentes
│   ├── relatorios/                 # Visualização e download de relatórios
│   └── auditoria/                  # Log de auditoria (admin)
├── layout/
│   └── main-layout/                # Layout principal com sidebar/navbar
└── shared/                         # Componentes e pipes reutilizáveis
```

---

## Endpoints da API

Todos os endpoints (exceto `/api/auth/login` e `/api/auth/registrar`) requerem autenticação via **Bearer Token JWT**.

| Método   | Rota                                          | Descrição                         |
| -------- | --------------------------------------------- | --------------------------------- |
| `POST`   | `/api/auth/login`                             | Autenticar e obter token JWT      |
| `POST`   | `/api/auth/registrar`                         | Registrar novo usuário            |
| `GET`    | `/api/obras`                                  | Listar todas as obras             |
| `GET`    | `/api/obras/{id}`                             | Detalhe de uma obra               |
| `GET`    | `/api/obras/por-status/{status}`              | Filtrar obras por status          |
| `GET`    | `/api/obras/por-profissional/{id}`            | Obras de um profissional          |
| `GET`    | `/api/obras/minhas`                           | Obras criadas pelo usuário logado |
| `POST`   | `/api/obras`                                  | Criar obra                        |
| `PUT`    | `/api/obras/{id}`                             | Atualizar obra                    |
| `DELETE` | `/api/obras/{id}`                             | Excluir obra                      |
| `GET`    | `/api/registrosdiarios/por-obra/{obraId}`     | Registros de uma obra             |
| `POST`   | `/api/registrosdiarios`                       | Criar registro diário             |
| `PUT`    | `/api/registrosdiarios/{id}`                  | Atualizar registro                |
| `DELETE` | `/api/registrosdiarios/{id}`                  | Excluir registro                  |
| `GET`    | `/api/assinaturas/por-entidade`               | Assinaturas de uma entidade       |
| `GET`    | `/api/assinaturas/pendentes`                  | Pendências do usuário logado      |
| `POST`   | `/api/assinaturas`                            | Registrar assinatura digital      |
| `GET`    | `/api/anexos/por-obra/{obraId}`               | Anexos de uma obra                |
| `POST`   | `/api/anexos`                                 | Fazer upload de anexo             |
| `DELETE` | `/api/anexos/{id}`                            | Excluir anexo                     |
| `GET`    | `/api/termosconclusao/por-obra/{obraId}`      | Termo de conclusão de uma obra    |
| `POST`   | `/api/termosconclusao`                        | Emitir termo de conclusão         |
| `PUT`    | `/api/termosconclusao/{id}`                   | Atualizar termo                   |
| `GET`    | `/api/relatorios/obra/{obraId}`               | Relatório completo da obra (JSON) |
| `GET`    | `/api/relatorios/obra/{obraId}/pdf`           | Download do relatório em PDF      |
| `GET`    | `/api/profissionais`                          | Listar profissionais              |
| `POST`   | `/api/profissionais`                          | Criar profissional                |
| `PUT`    | `/api/profissionais/{id}`                     | Atualizar profissional            |
| `DELETE` | `/api/profissionais/{id}`                     | Excluir profissional              |
| `GET`    | `/api/proprietarios`                          | Listar proprietários              |
| `POST`   | `/api/proprietarios`                          | Criar proprietário                |
| `PUT`    | `/api/proprietarios/{id}`                     | Atualizar proprietário            |
| `DELETE` | `/api/proprietarios/{id}`                     | Excluir proprietário              |
| `GET`    | `/api/usuarios`                               | Listar usuários                   |
| `POST`   | `/api/usuarios`                               | Criar usuário                     |
| `PUT`    | `/api/usuarios/{id}`                          | Atualizar usuário                 |
| `DELETE` | `/api/usuarios/{id}`                          | Excluir usuário                   |
| `GET`    | `/api/auditoria`                              | Log de auditoria paginado (Admin) |
| `GET`    | `/api/auditoria/por-usuario/{id}`             | Logs de um usuário (Admin)        |
| `GET`    | `/api/auditoria/por-entidade/{entidade}/{id}` | Logs de uma entidade (Admin)      |
| `GET`    | `/api/auditoria/por-periodo`                  | Logs por período (Admin)          |

---

## Papéis de Usuário

| Papel                | Descrição                                                            |
| -------------------- | -------------------------------------------------------------------- |
| `Administrador`      | Acesso total, incluindo auditoria e gestão de usuários               |
| `ResponsavelTecnico` | Cria e gerencia obras, registros diários, termos e assina documentos |
| `UsuarioCrea`        | Fiscal do CREA, pode visualizar e assinar documentos                 |
| `Proprietario`       | Acesso à sua própria obra e coleta de assinatura                     |

---

## Seed de Dados

Em ambiente de desenvolvimento, na primeira execução a API popula automaticamente o banco com dados de exemplo:

- Usuários para cada papel (Administrador, Responsável Técnico, Usuário CREA e Proprietário)
- Profissionais cadastrados
- Proprietários cadastrados
- Obras de exemplo com registros diários

As credenciais dos usuários de seed são exibidas nos logs da aplicação na inicialização.

---

## Variáveis de Ambiente

Para produção, substitua os valores do `appsettings.json` por variáveis de ambiente ou secrets seguros:

| Variável                               | Descrição                                                |
| -------------------------------------- | -------------------------------------------------------- |
| `ConnectionStrings__DefaultConnection` | String de conexão com o SQL Server                       |
| `Jwt__Key`                             | Chave secreta para assinar os tokens JWT (mín. 32 chars) |
| `Jwt__Issuer`                          | Emissor do token JWT                                     |
| `Jwt__Audience`                        | Audiência do token JWT                                   |
| `Jwt__ExpiracaoHoras`                  | Tempo de expiração do token em horas                     |

> As chaves JWT e strings de conexão **nunca devem ser versionadas** em repositórios públicos. Use `dotnet user-secrets` localmente ou variáveis de ambiente no servidor de produção.
