using LibrarySystem.Api.Common;
using LibrarySystem.Api.DTOs;
using LibrarySystem.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Api.Controllers;

/// <summary>
/// CRUD operations for library members.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    /// <summary>Gets every registered member.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberReadDto>>> GetAll()
    {
        var members = await _memberService.GetAllAsync();
        return Ok(members.Select(m => m.ToReadDto()));
    }

    /// <summary>Gets a single member by id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MemberReadDto>> GetById(int id)
    {
        var member = await _memberService.GetByIdAsync(id);
        if (member is null)
        {
            return NotFound();
        }

        return Ok(member.ToReadDto());
    }

    /// <summary>Registers a new member.</summary>
    [HttpPost]
    public async Task<ActionResult<MemberReadDto>> Create(MemberCreateDto dto)
    {
        var member = await _memberService.CreateAsync(dto.ToEntity());
        return CreatedAtAction(nameof(GetById), new { id = member.Id }, member.ToReadDto());
    }

    /// <summary>Updates an existing member's details.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, MemberUpdateDto dto)
    {
        var success = await _memberService.UpdateAsync(id, dto.ToEntity());
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>Deletes a member. Fails if the member has any loan history.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var success = await _memberService.DeleteAsync(id);
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
