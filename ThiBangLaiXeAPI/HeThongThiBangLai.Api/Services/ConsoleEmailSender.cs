using HeThongThiBangLai.Api.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HeThongThiBangLai.Api.Services;

public class ConsoleEmailSender : IEmailSender
{
    private readonly ILogger<ConsoleEmailSender> _logger;

    public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        _logger.LogInformation("[EmailSender] To: {ToEmail} | Subject: {Subject} | Body: {Body}", toEmail, subject, htmlBody);
        return Task.CompletedTask;
    }
}
