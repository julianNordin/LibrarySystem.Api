using LibrarySystem.Api.DTOs;
using LibrarySystem.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoanReadDto>>> GetAll()
    {
        var loans = await _loanService.GetAllAsync();
        return Ok(loans.Select(l => l.ToReadDto()));
    }

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

    [HttpPost("borrow")]
    public async Task<ActionResult<LoanReadDto>> Borrow(BorrowRequestDto dto)
    {
        var loan = await _loanService.BorrowAsync(dto.BookId, dto.MemberId);
        var readDto = (await _loanService.GetByIdAsync(loan.Id))!.ToReadDto();
        return CreatedAtAction(nameof(GetById), new { id = loan.Id }, readDto);
    }

    [HttpPost("{id:int}/return")]
    public async Task<ActionResult<LoanReadDto>> Return(int id)
    {
        var loan = await _loanService.ReturnAsync(id);
        var readDto = (await _loanService.GetByIdAsync(loan.Id))!.ToReadDto();
        return Ok(readDto);
    }
}
