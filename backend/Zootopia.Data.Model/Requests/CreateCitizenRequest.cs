namespace Zootopia.Biz.Model.Requests;

public class CreateCitizenRequest
{
    public string FullName { get; set; } = "";
    public string NationalId { get; set; } = "";
    public DateTime? DateOfBirth { get; set; }
    public string? AddressText { get; set; }
}
