namespace Zootopia.Biz.Model.Citizens;

public class UpdateCitizenRequest
{
    public string? FullName { get; set; }
    public string? NationalId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? AddressText { get; set; }
    public string? Password { get; set; }
    public bool? IsActive { get; set; }
}
