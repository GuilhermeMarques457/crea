using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using CREA.Application.Interfaces;

namespace CREA.API.Services;

public class MailjetEmailService(HttpClient httpClient, IConfiguration configuration, ILogger<MailjetEmailService> logger) : IEmailService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = null
    };

    public async Task EnviarAsync(string destinatarioEmail, string destinatarioNome, string assunto, string html, CancellationToken cancellationToken = default)
    {
        var apiKey = configuration["Mailjet:ApiKey"];
        var apiSecret = configuration["Mailjet:ApiSecret"];
        var senderEmail = configuration["Mailjet:SenderEmail"];
        var senderName = configuration["Mailjet:SenderName"] ?? "CREA";

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("A chave da API Mailjet não está configurada (Mailjet:ApiKey).");

        if (string.IsNullOrWhiteSpace(apiSecret))
            throw new InvalidOperationException("O segredo da API Mailjet não está configurado (Mailjet:ApiSecret).");

        if (string.IsNullOrWhiteSpace(senderEmail))
            throw new InvalidOperationException("O remetente Mailjet não está configurado (Mailjet:SenderEmail).");

        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey.Trim()}:{apiSecret.Trim()}"));
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.mailjet.com/v3.1/send");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        request.Headers.Accept.ParseAdd("application/json");
        request.Content = JsonContent.Create(new
        {
            Messages = new[]
            {
                new
                {
                    From = new { Email = senderEmail, Name = senderName },
                    To = new[] { new { Email = destinatarioEmail, Name = destinatarioNome } },
                    Subject = assunto,
                    HTMLPart = html
                }
            }
        }, options: JsonOptions);

        var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Falha ao enviar e-mail via Mailjet. Status {Status}. Corpo: {Body}", (int)response.StatusCode, body);
            throw new InvalidOperationException("Não foi possível enviar o e-mail. Verifique a configuração da API Mailjet e o remetente validado.");
        }
    }
}
