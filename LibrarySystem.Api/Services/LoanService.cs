using LibrarySystem.Api.Common;
using LibrarySystem.Api.Data;
using LibrarySystem.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Api.Services;

public class LoanService : ILoanService
{
    private const int LoanPeriodDays = 14;

    private readonly AppDbContext _context;
    private readonly IMemberService _memberService;

    public LoanService(AppDbContext context, IMemberService memberService)
    {
        _context = context;
        _memberService = memberService;
    }

    public async Task<IEnumerable<Loan>> GetAllAsync()
    {
        return await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Loan?> GetByIdAsync(int id)
    {
        return await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IEnumerable<Loan>> GetByMemberAsync(int memberId)
    {
        return await _context.Loans
            .Include(l => l.Book)
            .Where(l => l.MemberId == memberId)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<IEnumerable<Loan>> GetOverdueAsync() => throw new NotImplementedException();

    public async Task<Loan> BorrowAsync(int bookId, int memberId)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId)
            ?? throw new NotFoundException($"Book {bookId} not found.");

        var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == memberId)
            ?? throw new NotFoundException($"Member {memberId} not found.");

        var bookHasActiveLoan = await _context.Loans
            .AnyAsync(l => l.BookId == bookId && l.ReturnedDate == null);
        if (bookHasActiveLoan)
        {
            throw new BookNotAvailableException($"Book {bookId} is already on loan.");
        }

        var loan = new Loan
        {
            BookId = book.Id,
            MemberId = member.Id,
            BorrowedDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(LoanPeriodDays),
        };

        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();
        return loan;
    }

    public Task<Loan> ReturnAsync(int loanId) => throw new NotImplementedException();
}
