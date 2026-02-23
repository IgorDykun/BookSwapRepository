using AutoMapper;
using BookSwap.Application.Users.Commands.CreateUser;
using BookSwap.Application.Users.Dtos;
using BookSwap.Domain.Entities;

namespace BookSwap.Application.Users.Mapping
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserDocument, UserDto>().ReverseMap();
            CreateMap<CreateUserCommand, UserDocument>();
        }
    }
}
