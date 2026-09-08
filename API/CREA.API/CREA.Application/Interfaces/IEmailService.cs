namespace CREA.Application.Interfaces;

public interface IEmailService
{
    Task EnviarAsync(string destinatarioEmail, string destinatarioNome, string assunto, string html, CancellationToken cancellationToken = default);
}
