using Application.Interfaces;
using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace Infrastructure.Notifications
{
    public class EmailService(IFluentEmail fluentEmail) : IEmailService
    {
        public async Task<bool> SendEmailAsync(string email, string subject, string content)
        {
            SendResponse sendResponse = await fluentEmail.To(email)
                                                         .Subject(subject)
                                                         .Body(content)
                                                         .SendAsync();


            return sendResponse.Successful;
        }
    }
}
