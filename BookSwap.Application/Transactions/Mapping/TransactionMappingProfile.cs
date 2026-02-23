using AutoMapper;
using BookSwap.Application.Transactions.Commands.CreateTransaction;
using BookSwap.Application.Transactions.Dtos;
using BookSwap.Domain.Entities;

namespace BookSwap.Application.Transactions.Mapping
{
    public class TransactionMappingProfile : Profile
    {
        public TransactionMappingProfile()
        {
            CreateMap<TransactionDocument, TransactionDto>();
            CreateMap<CreateTransactionCommand, TransactionDocument>();
        }
    }
}
