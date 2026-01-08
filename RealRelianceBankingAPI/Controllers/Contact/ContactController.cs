using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RealRelianceBanking.Application.Contact.Commands;

namespace RealRelianceBankingAPI.Controllers.Contact
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        public ContactController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitContactForm(
            [FromBody] SendContactCommand request)
        {
            var result = await _mediator.Send(request);

            if (result.Success)
            {
                return Ok(new { message = result.Message });
            }

            return BadRequest(new { message = result.Message });
        }

        [HttpGet("test-email")]
        public async Task<IActionResult> TestEmail()
        {
            try
            {
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpUser = _configuration["EmailSettings:SmtpUser"];
                var fromEmail = _configuration["EmailSettings:FromEmail"];

                return Ok(new
                {
                    message = "Email settings loaded",
                    smtpHost = smtpHost,
                    smtpUser = smtpUser,
                    fromEmail = fromEmail,
                    passwordConfigured = !string.IsNullOrEmpty(_configuration["EmailSettings:SmtpPassword"])
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
