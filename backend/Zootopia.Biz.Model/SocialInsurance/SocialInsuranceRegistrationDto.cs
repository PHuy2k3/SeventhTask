namespace Zootopia.Biz.Model.SocialInsurance;

public class SocialInsuranceRegistrationDto
{
    public int Id { get; set; }
    public int CitizenId { get; set; }
    public string FullName { get; set; } = "";
    public string NationalId { get; set; } = "";
    public string SocialInsuranceNumber { get; set; } = "";
    public string SocialInsuranceProvider { get; set; } = "";
    public string Status { get; set; } = "";
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedBy { get; set; }
}