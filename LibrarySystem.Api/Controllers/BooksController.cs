using LibrarySystem.Api.DTOs;
using LibrarySystem.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookReadDto>>> GetAll()
    {
        var books = await _bookService.GetAllAsync();
        return Ok(books.Select(b => b.ToReadDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookReadDto>> GetById(int id)
    {
        var book = await _bookService.GetByIdAsync(id);
        if (book is null)
        {
            return NotFound();
        }

        return Ok(book.ToReadDto());
    }

    [HttpPost]
    public async Task<ActionResult<BookReadDto>> Create(BookCreateDto dto)
    {
        var book = await _bookService.CreateAsync(dto.ToEntity());
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book.ToReadDto());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, BookUpdateDto dto)
    {
        var success = await _bookService.UpdateAsync(id, dto.ToEntity());
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _bookService.DeleteAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}
