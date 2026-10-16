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
            <!DOCTYPE html>
            <html lang="pt-BR">
              <head>
                <meta charset="utf-8">
                <meta name="viewport" content="width=device-width, initial-scale=1">
                <meta name="color-scheme" content="light">
                <title>Assinatura pendente | Caderneta de Obras</title>
              </head>
              <body style="margin:0;padding:0;background-color:#f8fafc;font-family:Arial,Helvetica,sans-serif;color:#1e293b;">
                <div style="display:none;max-height:0;overflow:hidden;opacity:0;color:transparent;">
                  Há um documento aguardando sua assinatura na Caderneta de Obras.
                </div>
                <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0" style="background-color:#f8fafc;">
                  <tr>
                    <td align="center" style="padding:40px 16px;">
                      <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0" style="max-width:600px;background-color:#ffffff;border:1px solid #e2e8f0;border-radius:12px;overflow:hidden;">
                        <tr>
                          <td style="padding:24px 32px;background-color:#1565c0;">
                            <p style="margin:0;color:#ffffff;font-size:18px;font-weight:bold;letter-spacing:0.2px;">Caderneta de Obras</p>
                            <p style="margin:4px 0 0;color:#dbeafe;font-size:13px;">CREA</p>
                          </td>
                        </tr>
                        <tr>
                          <td style="padding:32px;">
                            <p style="margin:0 0 12px;color:#1565c0;font-size:12px;font-weight:bold;letter-spacing:1px;text-transform:uppercase;">Ação necessária</p>
                            <h1 style="margin:0 0 20px;color:#0f172a;font-size:24px;line-height:1.3;font-weight:700;">Documento aguardando sua assinatura</h1>
                            <p style="margin:0 0 20px;font-size:16px;line-height:1.6;">Olá, {nomeHtml}.</p>
                            <p style="margin:0 0 12px;font-size:15px;line-height:1.6;color:#475569;">Há um documento pendente de sua assinatura:</p>
                            <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0" style="margin:0 0 24px;background-color:#f8fafc;border:1px solid #e2e8f0;border-radius:8px;">
                              <tr>
                                <td style="padding:16px 20px;font-size:15px;line-height:1.5;font-weight:bold;color:#1e293b;">{tituloHtml}</td>
                              </tr>
                            </table>
                            <p style="margin:0 0 24px;font-size:15px;line-height:1.6;color:#475569;">Acesse o sistema para revisar o documento e concluir a assinatura. Se necessário, entre com sua conta para continuar.</p>
                            <table role="presentation" cellspacing="0" cellpadding="0" border="0" style="margin:0 auto 28px;">
                              <tr>
                                <td align="center" bgcolor="#1565c0" style="border-radius:8px;">
                                  <a href="{linkHtml}" style="display:inline-block;padding:14px 28px;border:1px solid #1565c0;border-radius:8px;color:#ffffff;font-size:15px;font-weight:bold;text-decoration:none;">Acessar documento</a>
                                </td>
                              </tr>
                            </table>
                            <p style="margin:0 0 8px;font-size:13px;line-height:1.5;color:#64748b;">Se o botão não funcionar, copie e cole este endereço no navegador:</p>
                            <p style="margin:0;word-break:break-all;font-size:13px;line-height:1.5;">
                              <a href="{linkHtml}" style="color:#1565c0;text-decoration:underline;">{linkHtml}</a>
                            </p>
                          </td>
                        </tr>
                        <tr>
                          <td style="padding:20px 32px;border-top:1px solid #e2e8f0;background-color:#f8fafc;">
                            <p style="margin:0;font-size:12px;line-height:1.6;color:#64748b;">Esta é uma mensagem automática da Caderneta de Obras. Se você não esperava este convite, pode desconsiderar este e-mail.</p>
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>
              </body>
            </html>
            """;
    }
}
