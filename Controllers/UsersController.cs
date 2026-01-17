using Inventory.API.Dtos;
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
    [Authorize(Policy = "ApenasCoordenador")]
    public async Task<ActionResult<IEnumerable<UserReadDto>>> GetUsers()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "GerenteOuSuperior")]
    public async Task<ActionResult<UserReadDto>> GetUser(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound("Utilizador não encontrado.");

        return Ok(user);
    }

    [HttpPost]
    [Authorize(Policy = "ApenasCoordenador")]
    public async Task<ActionResult<UserReadDto>> PostUser(UserCreateDto dto)
    {
        var response = await _userService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetUser), new { id = response.Id }, response);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "ApenasCoordenador")]
    public async Task<IActionResult> PutUser(int id, UserUpdateDto dto)
    {
        var success = await _userService.UpdateAsync(id, dto);
        if (!success) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "ApenasCoordenador")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var success = await _userService.DeleteAsync(id);
        if (!success) return NotFound();

        return NoContent();
    }
}