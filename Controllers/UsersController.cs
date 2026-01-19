using Inventory.API.Models.Dtos;
using Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Policy = "ApenasCoordenador")] // Apenas Coordenadores/Admins vêem todos
    public async Task<ActionResult<IEnumerable<UserReadDto>>> GetUsers()
    {
        return Ok(await _userService.GetAllAsync());
    }

    [HttpPost]
    [Authorize(Policy = "ApenasCoordenador")]
    public async Task<ActionResult<UserReadDto>> Create(UserCreateDto dto)
    {
        try
        {
            var result = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException)
        {
            return BadRequest("Role inválida fornecida.");
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "GerenteOuSuperior")]
    public async Task<ActionResult<UserReadDto>> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "ApenasCoordenador")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _userService.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}