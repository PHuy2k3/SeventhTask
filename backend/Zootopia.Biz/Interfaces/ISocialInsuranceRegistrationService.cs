using Zootopia.Biz.Model.SocialInsurance;

namespace Zootopia.Biz;

public interface ISocialInsuranceRegistrationService
{
    Task<SocialInsuranceRegistrationDto> CreateForCitizenAsync(string nationalId, CreateSocialInsuranceRegistrationRequest req);
    Task<SocialInsuranceRegistrationDto?> GetLatestForCitizenAsync(string nationalId);
    Task<List<SocialInsuranceRegistrationDto>> ListPendingAsync();
    Task<bool> ReviewAsync(int id, ReviewSocialInsuranceRegistrationRequest req, string reviewer);
}