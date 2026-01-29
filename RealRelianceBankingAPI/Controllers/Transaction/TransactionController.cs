using MediatR;
using Microsoft.AspNetCore.Mvc;
using RealRelianceBanking.Application.Transactions.Queries.GetAccountTransactions;
using RealRelianceBanking.Application.Transactions.Queries.GetTransactions;
using RealRelianceBanking.Application.Transactions.Queries;
using RealRelianceBanking.Contracts.Transactions.Transafer.TransferFundsCommand;
using RealRelianceBanking.Domain.Aggregates;
using Microsoft.AspNetCore.Authorization;
using RealRelianceBanking.Application.Transactions.Command.Update;
using RealRelianceBanking.Application.Transactions.Command.Create;

namespace RealRelianceBankingAPI.Controllers.Transaction
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ISender _mediator;

        public TransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetAccountTransactions")]
        public async Task<IActionResult> GetAccountTransactions([FromQuery] Guid accountId)
        {
            var transactions = await _mediator.Send(new GetAccountTransactionsQuery(accountId));
            return Ok(transactions);
        }

        [HttpPost]
        [Route("Transfer")]
        public async Task<IActionResult> TransferFunds([FromBody] TransferFundsCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result.Message);
        }

        [HttpGet("GetTransactions")]
        public async Task<ActionResult<IEnumerable<TransactionAggregate>>> GetTransactions([FromQuery] bool activeOnly = false)
        {
            var query = new GetTransactionsQuery(activeOnly);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("GetTransaction")]
        public async Task<ActionResult<TransactionAggregate>> GetDetailsByIdAsync([FromQuery] Guid id)
        {
            var query = new GetTransationDetailsById(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpPut("UpdateTransaction")]
        public async Task<IActionResult> UpdateTransaction([FromBody] UpdateTransactionCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
    }
}
