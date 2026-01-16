using Zootopia.Data.Model.Entities;

namespace Zootopia.Data;

public interface ISocialInsuranceRegistrationRepository
{
    Task<SocialInsuranceRegistration> CreateAsync(SocialInsuranceRegistration registration);
    Task<SocialInsuranceRegistration?> GetByIdAsync(int id);
    Task<SocialInsuranceRegistration?> GetByIdWithCitizenAsync(int id);
    Task<SocialInsuranceRegistration?> GetPendingByCitizenIdAsync(int citizenId);
    Task<List<SocialInsuranceRegistration>> ListPendingAsync();
    Task<List<SocialInsuranceRegistration>> ListByCitizenIdAsync(int citizenId);
    Task UpdateAsync(SocialInsuranceRegistration registration);
}