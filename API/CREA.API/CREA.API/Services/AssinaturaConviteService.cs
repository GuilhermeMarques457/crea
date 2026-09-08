using System.Net;
using CREA.Application.DTOs.Assinaturas;
using CREA.Application.Interfaces;
using CREA.Application.Interfaces.Repositories;
using CREA.Domain.Entities;
using CREA.Domain.Enums;

namespace CREA.API.Services;

public class AssinaturaConviteService(
    IAssinaturaRepository assinaturaRepository,
    IObraRepository obraRepository,
    IRelatoVisitaRepository relatoVisitaRepository,
    ITermoConclusaoRepository termoConclusaoRepository,
    IUsuarioRepository usuarioRepository,
    IEmailService emailService,
    IConfiguration configuration)
{
    public async Task<PreviewEmailAssinaturaDto?> ObterPreviewAsync(TipoEntidadeAssinatura tipoEntidade, Guid entidadeId, string email)
    {
        var contexto = await CarregarContextoAsync(tipoEntidade, entidadeId);
        if (contexto is null) return null;

        return new PreviewEmailAssinaturaDto
        {
            Titulo = contexto.Value.Titulo,
            Destinatarios = await ResolverDestinatariosPendentesAsync(tipoEntidade, entidadeId, contexto.Value.Obra, email)
        };
    }

    public async Task<(bool encontrado, string? erro)> EnviarAsync(EnviarEmailAssinaturaDto dto)
    {
        var contexto = await CarregarContextoAsync(dto.TipoEntidade, dto.EntidadeId);
        if (contexto is null) return (false, null);

        var destinatarios = await ResolverDestinatariosPendentesAsync(dto.TipoEntidade, dto.EntidadeId, contexto.Value.Obra, dto.EmailUsuarioLogado);
        if (destinatarios.Count == 0)
            return (true, "Não há destinatário pendente de assinatura com e-mail cadastrado.");

        var link = ResolverLink(dto.LinkAssinatura, dto.TipoEntidade, contexto.Value.Obra.Id, dto.EntidadeId);
        if (link is null)
            return (true, "O link de assinatura é inválido.");

        var titulo = contexto.Value.Titulo;
        foreach (var destinatario in destinatarios)
        {
            var html = MontarHtml(destinatario.Nome, titulo, link);
            await emailService.EnviarAsync(
                destinatario.Email,
                destinatario.Nome,
                $"Assinatura pendente — {titulo}",
                html);
        }

        return (true, null);
    }

    private async Task<(Obra Obra, string Titulo)?> CarregarContextoAsync(TipoEntidadeAssinatura tipoEntidade, Guid entidadeId)
    {
        switch (tipoEntidade)
        {
            case TipoEntidadeAssinatura.Obra:
            {
                var obra = await obraRepository.GetByIdWithDetailsAsync(entidadeId);
                if (obra is null) return null;
                return (obra, TituloObra(obra));
            }
            case TipoEntidadeAssinatura.RelatoVisita:
            {
                var relato = await relatoVisitaRepository.GetByIdWithDetailsAsync(entidadeId);
                if (relato is null) return null;
                var obra = await obraRepository.GetByIdWithDetailsAsync(relato.ObraId);
                if (obra is null) return null;
                return (obra, $"Relato de visita #{relato.NumeroSequencial}");
            }
            case TipoEntidadeAssinatura.TermoConclusao:
            {
                var termo = await termoConclusaoRepository.GetByIdWithDetailsAsync(entidadeId);
                if (termo is null) return null;
                var obra = await obraRepository.GetByIdWithDetailsAsync(termo.ObraId);
                if (obra is null) return null;
                return (obra, $"Termo de conclusão nº {termo.NumeroTermo}");
            }
            default:
                return null;
        }
    }

    private async Task<List<DestinatarioAssinaturaEmailDto>> ResolverDestinatariosPendentesAsync(
        TipoEntidadeAssinatura tipoEntidade,
        Guid entidadeId,
        Obra obra,
        string emailUsuarioLogado)
    {
        var existentes = (await assinaturaRepository.GetByEntidadeAsync(tipoEntidade, entidadeId))
            .Select(a => a.TipoAssinante)
            .ToHashSet();

        var requeridos = tipoEntidade == TipoEntidadeAssinatura.Obra
            ? new[] { TipoAssinante.Profissional, TipoAssinante.UsuarioCrea }
            : new[] { TipoAssinante.Profissional, TipoAssinante.Proprietario };

        var destinatarios = new List<DestinatarioAssinaturaEmailDto>();

        foreach (var tipo in requeridos)
        {
            if (existentes.Contains(tipo)) continue;

            if (tipo == TipoAssinante.Profissional)
            {
                var (nome, email) = await ResolverEmailProfissionalAsync(obra);
                AdicionarSeValido(destinatarios, nome, email, tipo);
            }
            else if (tipo == TipoAssinante.Proprietario)
            {
                var (nome, email) = await ResolverEmailProprietarioAsync(obra);
                AdicionarSeValido(destinatarios, nome, email, tipo);
            }
            else if (tipo == TipoAssinante.UsuarioCrea)
            {
                var usuariosCrea = await usuarioRepository.GetByTipoAsync(TipoUsuario.UsuarioCrea);
                foreach (var usuario in usuariosCrea)
                    AdicionarSeValido(destinatarios, usuario.Nome, usuario.Email, tipo);
            }
        }

        return destinatarios.Where(x => x.Email != emailUsuarioLogado).ToList();
    }

    private async Task<(string Nome, string? Email)> ResolverEmailProfissionalAsync(Obra obra)
    {
        if (obra.Profissional?.UsuarioId is Guid usuarioId)
        {
            var usuario = await usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario is not null)
                return (usuario.Nome, usuario.Email);
        }

        return (obra.Profissional?.Nome ?? "Profissional", obra.Profissional?.Email);
    }

    private async Task<(string Nome, string? Email)> ResolverEmailProprietarioAsync(Obra obra)
    {
        if (obra.Proprietario?.UsuarioId is Guid usuarioId)
        {
            var usuario = await usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario is not null)
                return (usuario.Nome, usuario.Email);
        }

        return (obra.Proprietario?.Nome ?? "Proprietário", obra.Proprietario?.Email);
    }

    private static void AdicionarSeValido(
        List<DestinatarioAssinaturaEmailDto> lista,
        string nome,
        string? email,
        TipoAssinante tipo)
    {
        if (string.IsNullOrWhiteSpace(email)) return;
        if (lista.Any(d => d.Email.Equals(email, StringComparison.OrdinalIgnoreCase))) return;

        lista.Add(new DestinatarioAssinaturaEmailDto
        {
            Nome = nome,
            Email = email.Trim(),
            TipoAssinante = tipo
        });
    }

    private string? ResolverLink(string? linkInformado, TipoEntidadeAssinatura tipoEntidade, Guid obraId, Guid entidadeId)
    {
        if (!string.IsNullOrWhiteSpace(linkInformado))
        {
            if (!Uri.TryCreate(linkInformado, UriKind.Absolute, out var uri)
                || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                return null;
            return uri.ToString();
        }

        var frontend = configuration["App:FrontendUrl"]?.TrimEnd('/');
        if (string.IsNullOrWhiteSpace(frontend)) return null;

        return tipoEntidade switch
        {
            TipoEntidadeAssinatura.Obra => $"{frontend}/obras/{obraId}?assinar=obra",
            TipoEntidadeAssinatura.RelatoVisita => $"{frontend}/obras/{obraId}?assinar=registro&r={entidadeId}",
            TipoEntidadeAssinatura.TermoConclusao => $"{frontend}/obras/{obraId}?assinar=termo",
            _ => null
        };
    }

    private static string TituloObra(Obra obra)
    {
        var caderneta = string.IsNullOrWhiteSpace(obra.NumeroCaderneta) ? null : $"#{obra.NumeroCaderneta} — ";
        return $"Obra {caderneta}{obra.LocalObra}";
    }

    private static string MontarHtml(string nome, string titulo, string link)
    {
        var nomeHtml = WebUtility.HtmlEncode(nome);
        var tituloHtml = WebUtility.HtmlEncode(titulo);
        var linkHtml = WebUtility.HtmlEncode(link);

        return $"""
            <html>
              <body style="font-family: Arial, Helvetica, sans-serif; color: #1e293b; line-height: 1.5;">
                <p>Olá, {nomeHtml}.</p>
                <p>Há um documento pendente de sua assinatura: <strong>{tituloHtml}</strong>.</p>
                <p>Clique no botão abaixo. Se você ainda não estiver autenticado, faça login — em seguida será redirecionado para a tela de assinatura.</p>
                <p style="margin: 28px 0;">
                  <a href="{linkHtml}"
                     style="display:inline-block;background:#1565c0;color:#ffffff;padding:12px 24px;border-radius:6px;text-decoration:none;font-weight:bold;">
                    Acessar e assinar
                  </a>
                </p>
                <p style="font-size: 12px; color: #64748b;">
                  Se o botão não funcionar, copie e cole este endereço no navegador:<br />
                  {linkHtml}
                </p>
              </body>
            </html>
            """;
    }
}
