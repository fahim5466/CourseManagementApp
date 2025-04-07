using Application.Interfaces;
using Application.Services;
using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace Infrastructure.Notifications
{
    public class EmailService(IFluentEmail fluentEmail, IHttpHelper httpHelper) : IEmailService
    {
        public async Task<bool> SendEmailAsync(string email, string subject, string content)
        {
            SendResponse sendResponse = await fluentEmail.To(email)
                                                         .Subject(subject)
                                                         .Body(content, true)
                                                         .SendAsync();


            return sendResponse.Successful;
        }

        public async Task SendEmailVerificationLinkAsync(string email, string verificationToken)
        {
            string href = $"{httpHelper.GetHostPathPrefix()}/api/{AuthService.VERIFY_EMAIL_ROUTE}?verificationToken={verificationToken}";
            await SendEmailAsync(email, "Verify your email", $"Please click this <a href={href}>link</a> to verify your email.");
        }
    }
}
