using BookSwap.Application.Transactions.Commands.CreateTransaction;
using BookSwap.Application.Transactions.Commands.DeleteTransaction;
using BookSwap.Application.Transactions.Commands.UpdateTransaction;
using BookSwap.Application.Transactions.Dtos;
using BookSwap.Application.Transactions.Queries.GetTransactionById;
using BookSwap.Application.Transactions.Queries.GetTransactionsList;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace BookSwap.Api.Controllers.Mongo
{
    [ApiController]
    [Route("api/mongo/[controller]")]
    public class TransactionMongoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionMongoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTransactionCommand command, CancellationToken cancellationToken)
        {
            var transaction = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
        {
            var query = new GetTransactionByIdQuery { Id = id };
            var transaction = await _mediator.Send(query, cancellationToken);
            if (transaction == null) return NotFound();
            return Ok(transaction);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var query = new GetTransactionsListQuery();
            var transactions = await _mediator.Send(query, cancellationToken);
            return Ok(transactions);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateTransactionCommand command)
        {
            command.Id = id;
            var updatedTransaction = await _mediator.Send(command);
            if (updatedTransaction == null) return NotFound();
            return Ok(updatedTransaction);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            var command = new DeleteTransactionCommand { Id = id };
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
    }
}
