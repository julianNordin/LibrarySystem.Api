using LibrarySystem.Api.Common;
using LibrarySystem.Api.DTOs;
using LibrarySystem.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Api.Controllers;

/// <summary>
/// CRUD operations for the book catalog.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    /// <summary>Gets every book in the catalog.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookReadDto>>> GetAll()
    {
        var books = await _bookService.GetAllAsync();
        return Ok(books.Select(b => b.ToReadDto()));
    }

    /// <summary>Gets a single book by id.</summary>
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

    /// <summary>Adds a new book to the catalog.</summary>
    [HttpPost]
    public async Task<ActionResult<BookReadDto>> Create(BookCreateDto dto)
    {
        var book = await _bookService.CreateAsync(dto.ToEntity());
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book.ToReadDto());
    }

    /// <summary>Updates an existing book's details.</summary>
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

    /// <summary>Deletes a book. Fails if the book has any loan history.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var success = await _bookService.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (DeleteConflictException ex)
        {
            return Conflict(ex.Message);
        }
    }
}
