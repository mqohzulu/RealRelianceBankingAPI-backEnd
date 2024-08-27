using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RealRelianceBankingAPI.Controllers.Transaction
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ISender _mediator;
        public TransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}
