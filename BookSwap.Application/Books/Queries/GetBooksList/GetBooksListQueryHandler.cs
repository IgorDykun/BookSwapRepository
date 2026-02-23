using AutoMapper;
using BookSwap.Application.Books.Dtos;
using BookSwap.Domain.Entities;
using BookSwap.Domain.Interfaces;
using BookSwap.Infrastructure.Repositories;
using MediatR;

namespace BookSwap.Application.Books.Queries.GetBooksList
{
    public class GetBooksListQueryHandler : IRequestHandler<GetBooksListQuery, List<BookDto>>
    {
        private readonly IBookRepository _repository;
        private readonly IMapper _mapper;

        public GetBooksListQueryHandler(IBookRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<BookDto>> Handle(GetBooksListQuery request, CancellationToken cancellationToken)
        {
            var books = await _repository.GetAllAsync();
            return _mapper.Map<List<BookDto>>(books);
        }
    }
}
