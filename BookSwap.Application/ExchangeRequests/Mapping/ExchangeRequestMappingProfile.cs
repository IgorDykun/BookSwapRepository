using AutoMapper;
using BookSwap.Application.ExchangeRequests.Commands.CreateExchangeRequest;
using BookSwap.Application.ExchangeRequests.Commands.UpdateExchangeRequest;
using BookSwap.Application.ExchangeRequests.Dtos;
using BookSwap.Domain.Entities;

namespace BookSwap.Application.ExchangeRequests.Mapping
{
    public class ExchangeRequestMappingProfile : Profile
    {
        public ExchangeRequestMappingProfile()
        {
            CreateMap<ExchangeRequestDocument, ExchangeRequestDto>();
            CreateMap<UserSummaryDocument, UserSummaryDto>();
            CreateMap<BookSummary, BookSummaryDto>();

            CreateMap<CreateExchangeRequestCommand, ExchangeRequestDocument>(); 
            CreateMap<UpdateExchangeRequestCommand, ExchangeRequestDocument>(); 
        }
    }
}
