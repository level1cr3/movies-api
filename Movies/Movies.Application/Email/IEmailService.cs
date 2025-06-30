namespace Movies.Application.Email;

public interface IEmailService
{
    Task SendAsync(string recipient, string subject, string htmlBody);
}