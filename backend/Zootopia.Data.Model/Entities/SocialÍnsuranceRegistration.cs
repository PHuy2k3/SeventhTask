namespace Zootopia.Data.Model.Entities;

public class SocialInsuranceRegistration
{
    public int Id { get; set; }
    public int CitizenId { get; set; }
    public string SocialInsuranceNumber { get; set; } = "";
    public string SocialInsuranceProvider { get; set; } = "";
    public string Status { get; set; } = "Pending";
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedBy { get; set; }

    public Citizen? Citizen { get; set; }
}