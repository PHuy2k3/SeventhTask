using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zootopia.Biz;
using Zootopia.Biz.Model.SocialInsurance;

namespace Zootopia.Api.Controllers;

[ApiController]
[Route("api/social-insurance-registrations")]
public class SocialInsuranceRegistrationsController : ControllerBase
{
    private readonly ISocialInsuranceRegistrationService _service;

    public SocialInsuranceRegistrationsController(ISocialInsuranceRegistrationService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Citizen")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSocialInsuranceRegistrationRequest req)
    {
        var nationalId = User.FindFirstValue(ClaimTypes.Name);
        if (string.IsNullOrWhiteSpace(nationalId))
            return Unauthorized(new { message = "Missing identity" });

        try
        {
            var dto = await _service.CreateForCitizenAsync(nationalId, req);
            return Ok(dto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Citizen")]
    [HttpGet("me/latest")]
    public async Task<IActionResult> GetLatestForMe()
    {
        var nationalId = User.FindFirstValue(ClaimTypes.Name);
        if (string.IsNullOrWhiteSpace(nationalId))
            return Unauthorized(new { message = "Missing identity" });

        var dto = await _service.GetLatestForCitizenAsync(nationalId);
        return Ok(dto);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("pending")]
    public async Task<IActionResult> ListPending()
        => Ok(await _service.ListPendingAsync());

    [Authorize(Roles = "Admin")]
    [HttpPost("{id:int}/review")]
    public async Task<IActionResult> Review([FromRoute] int id, [FromBody] SocialInsuranceDto req)
    {
        var reviewer = User.FindFirstValue(ClaimTypes.Name) ?? "Admin";

        try
        {
            var ok = await _service.ReviewAsync(id, req, reviewer);
            if (!ok) return NotFound(new { message = "Registration not found" });
            return Ok(new { message = "Reviewed" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}