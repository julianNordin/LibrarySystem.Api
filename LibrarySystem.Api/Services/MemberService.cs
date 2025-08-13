using LibrarySystem.Api.Data;
using LibrarySystem.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Api.Services;

public class MemberService : IMemberService
{
    private readonly AppDbContext _context;

    public MemberService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Member>> GetAllAsync()
    {
        return await _context.Members.AsNoTracking().ToListAsync();
    }

    public async Task<Member?> GetByIdAsync(int id)
    {
        return await _context.Members.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Member> CreateAsync(Member member)
    {
        _context.Members.Add(member);
        await _context.SaveChangesAsync();
        return member;
    }

    public async Task<bool> UpdateAsync(int id, Member member)
    {
        var existing = await _context.Members.FirstOrDefaultAsync(m => m.Id == id);
        if (existing is null)
        {
            return false;
        }

        existing.FullName = member.FullName;
        existing.Email = member.Email;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Members.FirstOrDefaultAsync(m => m.Id == id);
        if (existing is null)
        {
            return false;
        }

        _context.Members.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetActiveLoanCountAsync(int memberId)
    {
        return await _context.Loans.CountAsync(l => l.MemberId == memberId && l.ReturnedDate == null);
    }
}
