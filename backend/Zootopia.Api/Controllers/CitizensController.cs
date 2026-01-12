using Microsoft.AspNetCore.Mvc;
using Zootopia.Biz;
using Zootopia.Biz.Model.Citizens;
using Zootopia.Biz.Model.Requests;
using Microsoft.AspNetCore.Authorization;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
namespace Zootopia.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/citizens")]
public class CitizensController : ControllerBase
{
    private readonly ICitizenService _service;

    public CitizensController(ICitizenService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? q)
        => Ok(await _service.ListAsync(q));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCitizenRequest dto)
        => Ok(await _service.CreateAsync(dto));


    [HttpGet("export")]
    public async Task<IActionResult> Export([FromQuery] string? q = null)
    {
        // 1) lấy dữ liệu giống listCitizens (tùy service/repo của bạn)
        var items = await _service.ListAsync(q);
        // items: list citizen (Id, FullName, NationalId, DateOfBirth, AddressText...)

        // 2) tạo file Excel
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Citizens");

        // Header
        ws.Cell(1, 1).Value = "ID";
        ws.Cell(1, 2).Value = "Họ tên";
        ws.Cell(1, 3).Value = "CCCD/ID";
        ws.Cell(1, 4).Value = "Ngày sinh";
        ws.Cell(1, 5).Value = "Địa chỉ";

        ws.Range(1, 1, 1, 5).Style.Font.Bold = true;
        ws.Range(1, 1, 1, 5).Style.Fill.BackgroundColor = XLColor.FromHtml("#1f2937");
        ws.Range(1, 1, 1, 5).Style.Font.FontColor = XLColor.White;

        // Rows
        var row = 2;
        foreach (var c in items)
        {
            ws.Cell(row, 1).Value = c.Id;
            ws.Cell(row, 2).Value = c.FullName ?? "";
            ws.Cell(row, 3).Value = c.NationalId ?? "";
            ws.Cell(row, 4).Value = c.DateOfBirth?.ToString("yyyy-MM-dd") ?? "";
            ws.Cell(row, 5).Value = c.AddressText ?? "";
            row++;
        }

        ws.Columns().AdjustToContents();

        // 3) xuất file bytes
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        var bytes = ms.ToArray();

        var fileName = $"citizens_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName
        );
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var dto = await _service.GetByIdAsync(id);
        if (dto == null) return NotFound(new { message = "Citizen not found" });

        return Ok(dto);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCitizenRequest dto)
    {
        // validate nhẹ
        if (string.IsNullOrWhiteSpace(dto.FullName))
            return BadRequest(new { message = "FullName is required" });

        if (string.IsNullOrWhiteSpace(dto.NationalId))
            return BadRequest(new { message = "NationalId is required" });

        var ok = await _service.UpdateAsync(id, dto);
        if (!ok) return NotFound(new { message = "Citizen not found" });

        return Ok(new { message = "Updated" });
    }
}


