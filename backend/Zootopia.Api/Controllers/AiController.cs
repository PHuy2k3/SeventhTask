using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Zootopia.Api.Controllers;

[ApiController]
[Route("api/ai")]
public class AiController : ControllerBase
{
    private readonly IHttpClientFactory _http;

    public AiController(IHttpClientFactory http) => _http = http;

    [ApiExplorerSettings(IgnoreApi = true)] // ✅ Swagger bỏ qua endpoint này
    [HttpPost("extract")]
    public async Task<IActionResult> Extract([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "No file" });

        var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
        if (!allowed.Contains(file.ContentType))
            return BadRequest(new { error = "Only jpg/png/webp" });

        var client = _http.CreateClient("AiService");

        using var form = new MultipartFormDataContent();
        await using var stream = file.OpenReadStream();
        using var sc = new StreamContent(stream);
        sc.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        form.Add(sc, "file", file.FileName);

        var resp = await client.PostAsync("/extract", form);
        var json = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
            return StatusCode((int)resp.StatusCode, new { error = "AI service error", detail = json });

        return Content(json, "application/json");
    }
}
