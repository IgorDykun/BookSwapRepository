using MediatR;

namespace BookSwap.Application.Users.Commands.DeleteUser
{
    public class DeleteUserCommand : IRequest<Unit>
    {
        public string Id { get; set; } = string.Empty;
    }
}
