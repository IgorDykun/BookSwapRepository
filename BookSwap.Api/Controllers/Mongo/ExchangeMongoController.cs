using BookSwap.Application.ExchangeRequests.Commands.ConfirmExchangeRequest;
using BookSwap.Application.ExchangeRequests.Commands.CreateExchangeRequest;
using BookSwap.Application.ExchangeRequests.Commands.DeleteExchangeRequest;
using BookSwap.Application.ExchangeRequests.Commands.UpdateExchangeRequest;
using BookSwap.Application.ExchangeRequests.Queries.GetExchangeRequestById;
using BookSwap.Application.ExchangeRequests.Queries.GetExchangeRequestsList;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace BookSwap.Api.Controllers.Mongo
{
    [ApiController]
    [Route("api/mongo/[controller]")]
    public class ExchangeMongoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExchangeMongoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExchangeRequestCommand command, CancellationToken cancellationToken)
        {
            var exchangeRequest = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = exchangeRequest.Id }, exchangeRequest);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
        {
            var query = new GetExchangeRequestByIdQuery { Id = id };
            var exchangeRequest = await _mediator.Send(query, cancellationToken);
            if (exchangeRequest == null) return NotFound();
            return Ok(exchangeRequest);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var query = new GetExchangeRequestsListQuery();
            var exchangeRequests = await _mediator.Send(query, cancellationToken);
            return Ok(exchangeRequests);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateExchangeRequestCommand command)
        {
            command.Id = id;
            var updated = await _mediator.Send(command);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            var command = new DeleteExchangeRequestCommand { Id = id };
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpPut("{id}/confirm/{userId}")]
        public async Task<IActionResult> ConfirmRequest(string id, string userId, CancellationToken cancellationToken)
        {
            var command = new ConfirmExchangeRequestCommand { Id = id, UserId = userId };
            var confirmed = await _mediator.Send(command, cancellationToken);

            if (!confirmed) return NotFound();
            return NoContent();
        }
    }
}
