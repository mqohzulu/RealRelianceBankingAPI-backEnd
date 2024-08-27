using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RealRelianceBankingAPI.Controllers.Person
{
    [Route("api/[controller]")]
    [ApiController]

    public class PersonController : ControllerBase
    {
        private readonly ISender _mediator;
        public PersonController(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}
