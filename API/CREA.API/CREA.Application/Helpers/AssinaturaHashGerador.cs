using System.Security.Cryptography;
using System.Text;
using CREA.Domain.Enums;

namespace CREA.Application.Helpers;

public static class AssinaturaHashGerador
{
    public static string Gerar(
        TipoEntidadeAssinatura tipoEntidade,
        Guid entidadeId,
        Guid usuarioId,
        TipoAssinante tipoAssinante,
        DateTime data)
        => Gerar(tipoEntidade, entidadeId, usuarioId, tipoAssinante, string.Empty, string.Empty, data);

    public static string Gerar(
        TipoEntidadeAssinatura tipoEntidade,
        Guid entidadeId,
        Guid usuarioId,
        TipoAssinante tipoAssinante,
        string nome,
        string ip,
        DateTime data)
    {
        var conteudo = $"ASSINATURA:{tipoEntidade}:{entidadeId}:{usuarioId}:{tipoAssinante}:{nome}:{ip}:{data:O}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(conteudo));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
