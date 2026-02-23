using MediatR;
using BookSwap.Application.Users.Commands.CreateUser;
using BookSwap.Application.Users.Commands.UpdateUser;
using BookSwap.Application.Users.Commands.DeleteUser;
using BookSwap.Application.Users.Queries.GetUserById;
using BookSwap.Application.Users.Queries.GetUsersList;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace BookSwap.Api.Controllers.Mongo
{
    [ApiController]
    [Route("api/mongo/[controller]")]
    public class UserMongoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserMongoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
        {
            var query = new GetUserByIdQuery { Id = id };
            var user = await _mediator.Send(query, cancellationToken);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var query = new GetUsersListQuery();
            var users = await _mediator.Send(query, cancellationToken);
            return Ok(users);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserCommand command)
        {
            command.Id = id;
            var updatedUser = await _mediator.Send(command);
            if (updatedUser == null) return NotFound();
            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            var command = new DeleteUserCommand { Id = id };
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
    }
}
