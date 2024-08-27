using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }
}
