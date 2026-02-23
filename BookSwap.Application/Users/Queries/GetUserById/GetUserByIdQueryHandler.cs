using AutoMapper;
using BookSwap.Application.Users.Dtos;
using BookSwap.Domain.Entities;
using BookSwap.Domain.Interfaces;
using BookSwap.Infrastructure.Repositories;
using MediatR;

namespace BookSwap.Application.Users.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(request.Id);
            if (user == null)
                throw new Exception("User not found");

            return _mapper.Map<UserDto>(user);
        }
    }
}
