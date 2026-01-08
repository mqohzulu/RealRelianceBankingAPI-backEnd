using Microsoft.Extensions.Configuration;
using RealRelianceBanking.Application.Common.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(
         string to,
         string subject,
         string body,
         bool isHtml = false,
         CancellationToken cancellationToken = default)
        {
            using var client = new SmtpClient(
                _configuration["EmailSettings:SmtpHost"],
                int.Parse(_configuration["EmailSettings:SmtpPort"])
            )
            {
                Credentials = new NetworkCredential(
                    _configuration["EmailSettings:SmtpUser"],
                    _configuration["EmailSettings:SmtpPassword"]
                ),
                EnableSsl = true,
                UseDefaultCredentials = false
            };

            var message = new MailMessage
            {
                From = new MailAddress(
                    _configuration["EmailSettings:FromEmail"],
                    _configuration["EmailSettings:FromName"]
                ),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            message.To.Add(to);

            await client.SendMailAsync(message, cancellationToken);
        }

    }
}
