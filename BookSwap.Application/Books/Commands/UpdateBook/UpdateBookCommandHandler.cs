using AutoMapper;
using BookSwap.Application.Books.Dtos;
using BookSwap.Domain.Entities;
using BookSwap.Domain.Interfaces;
using BookSwap.Infrastructure.Repositories;
using MediatR;

namespace BookSwap.Application.Books.Commands.UpdateBook
{
    public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, BookDto>
    {
        private readonly IBookRepository _repository;
        private readonly IMapper _mapper;

        public UpdateBookCommandHandler(IBookRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<BookDto> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _repository.GetByIdAsync(request.Id);
            if (book == null)
                throw new KeyNotFoundException("Book not found");

            book.Title = request.Title;
            book.Author = request.Author;

            await _repository.UpdateAsync(request.Id, book);

            return _mapper.Map<BookDto>(book);
        }
    }
}
