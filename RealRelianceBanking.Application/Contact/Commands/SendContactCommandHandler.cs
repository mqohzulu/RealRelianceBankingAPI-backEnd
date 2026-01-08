using MediatR;
using Microsoft.Extensions.Logging;
using RealRelianceBanking.Application.Common.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Contact.Commands
{
    public class SendContactFormCommandHandler
       : IRequestHandler<SendContactCommand, SendContactResult>
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<SendContactFormCommandHandler> _logger;

        public SendContactFormCommandHandler(
            IEmailService emailService,
            ILogger<SendContactFormCommandHandler> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<SendContactResult> Handle(
            SendContactCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var emailContent = BuildEmailContent(request);

                await _emailService.SendEmailAsync(
                    to: "mqohzulu@gmail.com",
                    subject: $"Contact Form: {request.Subject}",
                    body: emailContent,
                    isHtml: true,
                    cancellationToken: cancellationToken
                );

                _logger.LogInformation(
                    "Contact form email sent successfully from {Email}",
                    request.Email
                );

                return new SendContactResult(
                    Success: true,
                    Message: "Your message has been sent successfully!"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send contact form email from {Email}",
                    request.Email
                );

                return new SendContactResult(
                    Success: false,
                    Message: "Failed to send message. Please try again later."
                );
            }
        }

        private string BuildEmailContent(SendContactCommand request)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>New enquiry from Website  Submission</h2>
                    <hr/>
                    <p><strong>From:</strong> {request.Name}</p>
                    <p><strong>Email:</strong> {request.Email}</p>
                    <p><strong>Subject:</strong> {request.Subject}</p>
                    <hr/>
                    <h3>Message:</h3>
                    <p>{request.Message.Replace("\n", "<br/>")}</p>
                    <hr/>
                    <p style='color: #666; font-size: 12px;'>
                        Sent from Real Reliance Banking Contact
                    </p>
                </body>
                </html>
            ";
        }
    }
}
