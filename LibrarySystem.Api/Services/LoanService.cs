using LibrarySystem.Api.Data;
using LibrarySystem.Api.Domain;

namespace LibrarySystem.Api.Services;

public class LoanService : ILoanService
{
    private readonly AppDbContext _context;
    private readonly IMemberService _memberService;

    public LoanService(AppDbContext context, IMemberService memberService)
    {
        _context = context;
        _memberService = memberService;
    }

    public Task<IEnumerable<Loan>> GetAllAsync() => throw new NotImplementedException();

    public Task<Loan?> GetByIdAsync(int id) => throw new NotImplementedException();

    public Task<IEnumerable<Loan>> GetByMemberAsync(int memberId) => throw new NotImplementedException();

    public Task<IEnumerable<Loan>> GetOverdueAsync() => throw new NotImplementedException();

    public Task<Loan> BorrowAsync(int bookId, int memberId) => throw new NotImplementedException();

    public Task<Loan> ReturnAsync(int loanId) => throw new NotImplementedException();
}
