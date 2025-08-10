using LibrarySystem.Api.Data;
using LibrarySystem.Api.Domain;

namespace LibrarySystem.Api.Services;

public class BookService : IBookService
{
    private readonly AppDbContext _context;

    public BookService(AppDbContext context)
    {
        _context = context;
    }

    public Task<IEnumerable<Book>> GetAllAsync() => throw new NotImplementedException();

    public Task<Book?> GetByIdAsync(int id) => throw new NotImplementedException();

    public Task<Book> CreateAsync(Book book) => throw new NotImplementedException();

    public Task<bool> UpdateAsync(int id, Book book) => throw new NotImplementedException();

    public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
}
