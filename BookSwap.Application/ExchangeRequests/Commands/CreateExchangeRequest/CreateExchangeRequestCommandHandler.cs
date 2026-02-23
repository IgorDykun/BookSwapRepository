using AutoMapper;
using BookSwap.Application.ExchangeRequests.Dtos;
using BookSwap.Domain.Entities;
using BookSwap.Domain.Interfaces;
using BookSwap.Infrastructure.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookSwap.Application.ExchangeRequests.Commands.CreateExchangeRequest
{
    public class CreateExchangeRequestCommandHandler : IRequestHandler<CreateExchangeRequestCommand, ExchangeRequestDto>
    {
        private readonly IExchangeRequestRepository _repository;
        private readonly IMapper _mapper;

        public CreateExchangeRequestCommandHandler(IExchangeRequestRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ExchangeRequestDto> Handle(CreateExchangeRequestCommand request, CancellationToken cancellationToken)
        {
            var exchangeRequest = new ExchangeRequestDocument
            {
                FromUser = new UserSummaryDocument { Id = request.FromUserId, Name = request.FromUserName },
                ToUser = new UserSummaryDocument { Id = request.ToUserId, Name = request.ToUserName },
                BookOffered = new BookSummary { Id = request.BookOfferedId, Title = request.BookOfferedTitle },
                BookRequested = new BookSummary { Id = request.BookRequestedId, Title = request.BookRequestedTitle },
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.CreateAsync(exchangeRequest);

            return _mapper.Map<ExchangeRequestDto>(exchangeRequest);
        }
    }
}
