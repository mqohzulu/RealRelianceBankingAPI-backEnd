using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RealRelianceBankingAPI.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly ISender _mediator;
        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
