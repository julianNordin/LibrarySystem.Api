using LibrarySystem.Api.Data;
using LibrarySystem.Api.Domain;

namespace LibrarySystem.Api.Services;

public class MemberService : IMemberService
{
    private readonly AppDbContext _context;

    public MemberService(AppDbContext context)
    {
        _context = context;
    }

    public Task<IEnumerable<Member>> GetAllAsync() => throw new NotImplementedException();

    public Task<Member?> GetByIdAsync(int id) => throw new NotImplementedException();

    public Task<Member> CreateAsync(Member member) => throw new NotImplementedException();

    public Task<bool> UpdateAsync(int id, Member member) => throw new NotImplementedException();

    public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();

    public Task<int> GetActiveLoanCountAsync(int memberId) => throw new NotImplementedException();
}
