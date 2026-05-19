namespace CREA.API.Helpers;

public static class UserAgentInfo
{
    public static (string? Navegador, string? SistemaOperacional, string? Dispositivo) Parse(string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
            return (null, null, null);

        string? navegador = null;
        if (userAgent.Contains("Edg/", StringComparison.OrdinalIgnoreCase))
            navegador = "Microsoft Edge";
        else if (userAgent.Contains("Chrome/", StringComparison.OrdinalIgnoreCase))
            navegador = "Google Chrome";
        else if (userAgent.Contains("Firefox/", StringComparison.OrdinalIgnoreCase))
            navegador = "Mozilla Firefox";
        else if (userAgent.Contains("Safari/", StringComparison.OrdinalIgnoreCase))
            navegador = "Safari";

        string? sistema = null;
        if (userAgent.Contains("Windows", StringComparison.OrdinalIgnoreCase))
            sistema = "Windows";
        else if (userAgent.Contains("Mac OS", StringComparison.OrdinalIgnoreCase))
            sistema = "macOS";
        else if (userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase))
            sistema = "Android";
        else if (userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase) || userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase))
            sistema = "iOS";
        else if (userAgent.Contains("Linux", StringComparison.OrdinalIgnoreCase))
            sistema = "Linux";

        string? dispositivo = null;
        if (userAgent.Contains("Mobile", StringComparison.OrdinalIgnoreCase))
            dispositivo = "Mobile";
        else if (userAgent.Contains("Tablet", StringComparison.OrdinalIgnoreCase) || userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase))
            dispositivo = "Tablet";
        else
            dispositivo = "Desktop";

        return (navegador, sistema, dispositivo);
    }
}
