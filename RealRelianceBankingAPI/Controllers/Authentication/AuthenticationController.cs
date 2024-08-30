using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealRelianceBanking.Application.Authentication.Queries.Login;
using RealRelianceBanking.Application.Common.Errors;

namespace RealRelianceBankingAPI.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private readonly ISender _mediator;
        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginQuery loginRequest)
        {
            var query = new LoginQuery(loginRequest.Email, loginRequest.Password);

            try
            {
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (InvalidUser)
            {
                return Unauthorized("Invalid user.");
            }
            catch (InvalidPassword)
            {
                return Unauthorized("Invalid password.");
            }
        }
    }
}
