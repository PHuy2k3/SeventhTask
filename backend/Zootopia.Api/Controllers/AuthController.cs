using Microsoft.AspNetCore.Mvc;
using Zootopia.Api.Helpers;
using Zootopia.Api.Models;

namespace Zootopia.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        var admin = _config.GetSection("AdminAccount");

        if (req.Username != admin["Username"] ||
            req.Password != admin["Password"])
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var token = JwtHelper.GenerateToken(
            username: req.Username,
            role: "Admin",
            _config
        );

        return Ok(new
        {
            token,
            role = "Admin"
        });
    }
}
