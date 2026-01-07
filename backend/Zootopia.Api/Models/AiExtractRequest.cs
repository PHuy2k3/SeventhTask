using Microsoft.AspNetCore.Mvc;

namespace Zootopia.Api.Models;

public class AiExtractRequest
{
    [FromForm(Name = "file")]
    public IFormFile File { get; set; } = default!;
}
