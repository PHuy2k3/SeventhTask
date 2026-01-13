namespace Zootopia.Data.Model.Entities;

public class Citizen
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string NationalId { get; set; } = "";
    public DateTime? DateOfBirth { get; set; }
    public string? AddressText { get; set; }
    public string? PasswordHash { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
