using AutoMapper;
using BookSwap.Application.Books.Commands.CreateBook;
using BookSwap.Application.Books.Commands.UpdateBook;
using BookSwap.Application.Books.Dtos;
using BookSwap.Domain.Entities;

public class BookMappingProfile : Profile
{
    public BookMappingProfile()
    {
        CreateMap<BookDocument, BookDto>()
            .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner));
        CreateMap<UserSummaryDocument, UserSummaryDto>();

        CreateMap<BookDto, BookDocument>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner));
        CreateMap<UserSummaryDto, UserSummaryDocument>();

        CreateMap<CreateBookCommand, BookDocument>()
            .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => new UserSummaryDocument
            {
                Id = src.OwnerId,
                Name = src.OwnerName
            }));
        CreateMap<UpdateBookCommand, BookDocument>(); 

    }
}
