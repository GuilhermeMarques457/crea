using System.Security.Cryptography;
using System.Text;

namespace CREA.Application.Helpers;

public static class SenhaHashHelper
{
    public static string Hash(string senha)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
