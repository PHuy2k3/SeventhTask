namespace Zootopia.Biz.Model.Ai;

public class OcrResultDto
{
    public string? FullName { get; set; }
    public string? NationalId { get; set; }
    public string? Dob { get; set; } // dd/MM/yyyy hoặc yyyy-MM-dd
    public string? AddressText { get; set; }
    public double Confidence { get; set; }
    public string? RawText { get; set; }
}
