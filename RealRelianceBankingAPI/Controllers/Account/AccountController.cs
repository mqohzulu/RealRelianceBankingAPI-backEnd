using MediatR;
using Microsoft.AspNetCore.Mvc;
using RealRelianceBanking.Application.Transactions.Queries.GetAccountTransactions;
using RealRelianceBanking.Application.Accounts.Queries.GetAccountById;
using RealRelianceBanking.Application.Accounts.Commands.DeleteAccount;
using RealRelianceBanking.Application.Accounts.Queries.GetAccounts;
using RealRelianceBanking.Application.Accounts.Commands.AddAccount;
using RealRelianceBanking.Application.Accounts.Commands.CloseAccountCommand;
using Microsoft.AspNetCore.Authorization;
using RealRelianceBanking.Application.Accounts.Queries.GetAccountsByPersonId;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace RealRelianceBankingAPI.Controllers.Account
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly ISender _mediator;
        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetAccountsByPersonId")]
        public async Task<IActionResult> GetAccountsByPersonId([FromQuery] Guid personId)
        {
            var accounts = await _mediator.Send(new GetAccountsByPersonIdQuery(personId));
            return Ok(accounts);
        }

        [HttpGet("getAccountById")]
        public async Task<IActionResult> getAccountById([FromQuery] Guid accountId)
        {
            var accounts = await _mediator.Send(new GetAccountByIdQuery(accountId));
            return Ok(accounts);
        }

        [HttpDelete("DeleteAccount")]
        public async Task<IActionResult> DeleteAccount([FromQuery] DeleteAccountCommand request)
        {
            await _mediator.Send(new DeleteAccountCommand(request.AccountId));
            return NoContent();
        }
        [HttpGet("GetAccounts")]
        public async Task<IActionResult> GetAccounts([FromQuery] bool activeOnly = true)
        {
            var result = await _mediator.Send(new GetAccountsQuery(activeOnly));
            return Ok(result);
        }

        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountCommand command)
        {
            try
            {
                var accountId = await _mediator.Send(command);
                return Ok(accountId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("CloseAccount")]
        public async Task<IActionResult> CloseAccount([FromBody] CloseAccountCommand request)
        {
            try
            {
                var command = new CloseAccountCommand(request.AccountId);
                var result = await _mediator.Send(command);

                if (result)
                {
                    return Ok("Account closed successfully");
                }
                else
                {
                    return BadRequest("Failed to close the account");
                }
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
