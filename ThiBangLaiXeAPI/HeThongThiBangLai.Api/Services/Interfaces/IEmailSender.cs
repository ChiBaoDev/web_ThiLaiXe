namespace HeThongThiBangLai.Api.Services.Interfaces;

public interface IEmailSender
{
    Task SendAsync(string toEmail, string subject, string htmlBody);
}
