using System.Net.Http.Json;
using System.Text.Json;
using CREA.Application.Interfaces;

namespace CREA.API.Services;

public class BrevoEmailService(HttpClient httpClient, IConfiguration configuration, ILogger<BrevoEmailService> logger) : IEmailService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task EnviarAsync(string destinatarioEmail, string destinatarioNome, string assunto, string html, CancellationToken cancellationToken = default)
    {
        var apiKey = configuration["Brevo:ApiKey"];
        var senderEmail = configuration["Brevo:SenderEmail"];
        var senderName = configuration["Brevo:SenderName"] ?? "CREA";

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("A chave da API Brevo não está configurada (Brevo:ApiKey).");

        if (string.IsNullOrWhiteSpace(senderEmail))
            throw new InvalidOperationException("O remetente Brevo não está configurado (Brevo:SenderEmail).");

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");
        request.Headers.TryAddWithoutValidation("api-key", apiKey);
        request.Headers.Accept.ParseAdd("application/json");
        request.Content = JsonContent.Create(new
        {
            sender = new { name = senderName, email = senderEmail },
            to = new[] { new { email = destinatarioEmail, name = destinatarioNome } },
            subject = assunto,
            htmlContent = html
        }, options: JsonOptions);

        var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Falha ao enviar e-mail via Brevo. Status {Status}. Corpo: {Body}", (int)response.StatusCode, body);
            throw new InvalidOperationException("Não foi possível enviar o e-mail. Verifique a configuração do Brevo e o remetente verificado.");
        }
    }
}
