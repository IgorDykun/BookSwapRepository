using AutoMapper;
using BookSwap.Application.Users.Dtos;
using BookSwap.Domain.Entities;
using BookSwap.Domain.Interfaces;
using BookSwap.Infrastructure.Repositories;
using MediatR;

namespace BookSwap.Application.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _repository.GetByIdAsync(request.Id);
            if (existingUser == null)
                throw new Exception("User not found");

            _mapper.Map(request, existingUser);
            await _repository.UpdateAsync(request.Id, existingUser);

            return _mapper.Map<UserDto>(existingUser);
        }
    }
}
