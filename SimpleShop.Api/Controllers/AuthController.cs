using Microsoft.AspNetCore.Mvc;
using SimpleShop.Api.Models;
using SimpleShop.Api.Services;

namespace SimpleShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _service;

    public AuthController(AuthService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<ActionResult<object>> Register([FromBody] RegisterUserDto dto)
    {
        try
        {
            var created = await _service.RegisterAsync(dto);
            return Created($"/api/auth/{created.Id}", new
            {
                created.Id,
                created.Username,
                created.Email
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<object>> Login([FromBody] LoginDto dto)
    {
        try
        {
            var user = await _service.LoginAsync(dto);
            if (user is null)
                return Unauthorized(new { error = "Invalid email or password." });

            return Ok(new
            {
                message = "Login successful.",
                user.Id,
                user.Username,
                user.Email
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}