using MediatR;
using BookSwap.Application.Users.Dtos;
using System.Collections.Generic;

namespace BookSwap.Application.Users.Queries.GetUsersList
{
    public class GetUsersListQuery : IRequest<List<UserDto>>
    {
    }
}
