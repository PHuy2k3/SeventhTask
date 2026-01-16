using Microsoft.AspNetCore.Mvc;
using Zootopia.Api.Helpers;
using Zootopia.Api.Models;
using Zootopia.Biz.Interfaces;

namespace Zootopia.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ICitizenService _citizenService;

    public AuthController(IConfiguration config, ICitizenService citizenService)
    {
        _config = config;
        _citizenService = citizenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var admin = _config.GetSection("AdminAccount");

        if (req.Username != admin["Username"] ||
            req.Password != admin["Password"])
        {
            return await LoginCitizenAsync(req);
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

    private async Task<IActionResult> LoginCitizenAsync(LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Username) ||
            string.IsNullOrWhiteSpace(req.Password))
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var citizen = await _citizenService.AuthenticateCitizenAsync(req.Username, req.Password);
        if (citizen == null)
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var token = JwtHelper.GenerateToken(
            username: citizen.NationalId,
            role: "Citizen",
            _config
        );

        return Ok(new
        {
            token,
            role = "Citizen"
        });
    }
}