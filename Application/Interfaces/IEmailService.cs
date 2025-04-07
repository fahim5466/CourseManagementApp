namespace Application.Interfaces
{
    public interface IEmailService
    {
        public Task<bool> SendEmailAsync(string email, string subject, string content);
        public Task SendEmailVerificationLinkAsync(string email, string verificationToken);
    }
}
