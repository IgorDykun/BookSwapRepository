using AutoMapper;
using BookSwap.Application.Books.Dtos;
using BookSwap.Domain.Entities;
using BookSwap.Domain.Interfaces;
using BookSwap.Infrastructure.Repositories;
using MediatR;

namespace BookSwap.Application.Books.Commands.CreateBook
{
    public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookDto>
    {
        private readonly IBookRepository _repository;
        private readonly IMapper _mapper;

        public CreateBookCommandHandler(IBookRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            var book = new BookDocument
            {
                Title = request.Title,
                Author = request.Author,
                Owner = new UserSummaryDocument
                {
                    Id = request.OwnerId,
                    Name = request.OwnerName
                }
            };

            await _repository.CreateAsync(book);

            return _mapper.Map<BookDto>(book);
        }
    }
}
