using MediatR;
using BookSwap.Application.Users.Dtos;

namespace BookSwap.Application.Users.Queries.GetUserById
{
    public class GetUserByIdQuery : IRequest<UserDto>
    {
        public string Id { get; set; } = string.Empty;
    }
}
