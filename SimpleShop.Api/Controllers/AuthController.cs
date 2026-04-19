using Microsoft.AspNetCore.Mvc;
using SimpleShop.Api.Models;
using SimpleShop.Api.Services;

namespace SimpleShop.Api.Controllers;

/// <summary>
/// Provides endpoints for user registration and login.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="service">Service responsible for authentication logic.</param>
    public AuthController(AuthService service)
    {
        _service = service;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="dto">The registration data submitted by the client.</param>
    /// <returns>
    /// A <c>201 Created</c> response with the created user information when successful,
    /// or a <c>400 Bad Request</c> response if the input is invalid.
    /// </returns>
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
            // Return a client-friendly validation error if registration input is invalid.
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Logs in an existing user.
    /// </summary>
    /// <param name="dto">The login credentials provided by the client.</param>
    /// <returns>
    /// A <c>200 OK</c> response when login succeeds,
    /// a <c>401 Unauthorized</c> response when credentials are invalid,
    /// or a <c>400 Bad Request</c> response when the input is malformed.
    /// </returns>
    [HttpPost("login")]
    public async Task<ActionResult<object>> Login([FromBody] LoginDto dto)
    {
        try
        {
            var user = await _service.LoginAsync(dto);

            if (user is null)
            {
                return Unauthorized(new { error = "Invalid email or password." });
            }

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
            // Return a client-friendly validation error if login input is invalid.
            return BadRequest(new { error = ex.Message });
        }
    }
}