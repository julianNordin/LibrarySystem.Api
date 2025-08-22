using LibrarySystem.Api.Common;
using LibrarySystem.Api.DTOs;
using LibrarySystem.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Api.Controllers;

/// <summary>
/// Borrowing, returning, and querying loans.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    /// <summary>Gets every loan (active and returned).</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoanReadDto>>> GetAll()
    {
        var loans = await _loanService.GetAllAsync();
        return Ok(loans.Select(l => l.ToReadDto()));
    }

    /// <summary>Gets a single loan by id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LoanReadDto>> GetById(int id)
    {
        var loan = await _loanService.GetByIdAsync(id);
        if (loan is null)
        {
            return NotFound();
        }

        return Ok(loan.ToReadDto());
    }

    /// <summary>Gets every loan that is past its due date and not yet returned.</summary>
    [HttpGet("overdue")]
    public async Task<ActionResult<IEnumerable<LoanReadDto>>> GetOverdue()
    {
        var loans = await _loanService.GetOverdueAsync();
        return Ok(loans.Select(l => l.ToReadDto()));
    }

    /// <summary>Gets every loan (active and returned) for one member.</summary>
    [HttpGet("member/{memberId:int}")]
    public async Task<ActionResult<IEnumerable<LoanReadDto>>> GetByMember(int memberId)
    {
        var loans = await _loanService.GetByMemberAsync(memberId);
        return Ok(loans.Select(l => l.ToReadDto()));
    }

    /// <summary>
    /// Borrows a book for a member. Rejected if the book is already on loan or the
    /// member is at their active-loan cap.
    /// </summary>
    [HttpPost("borrow")]
    public async Task<ActionResult<LoanReadDto>> Borrow(BorrowRequestDto dto)
    {
        try
        {
            var loan = await _loanService.BorrowAsync(dto.BookId, dto.MemberId);
            var readDto = (await _loanService.GetByIdAsync(loan.Id))!.ToReadDto();
            return CreatedAtAction(nameof(GetById), new { id = loan.Id }, readDto);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (BookNotAvailableException ex)
        {
            return Conflict(ex.Message);
        }
        catch (LoanLimitExceededException ex)
        {
            return Conflict(ex.Message);
        }
    }

    /// <summary>Returns a borrowed book. Rejected if the loan was already returned.</summary>
    [HttpPost("{id:int}/return")]
    public async Task<ActionResult<LoanReadDto>> Return(int id)
    {
        try
        {
            var loan = await _loanService.ReturnAsync(id);
            var readDto = (await _loanService.GetByIdAsync(loan.Id))!.ToReadDto();
            return Ok(readDto);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (LoanAlreadyReturnedException ex)
        {
            return Conflict(ex.Message);
        }
    }
}
