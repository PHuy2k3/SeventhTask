using Zootopia.Biz;
using Zootopia.Biz.Model.SocialInsurance;
using Zootopia.Data;
using Zootopia.Data.Model.Entities;

namespace Zootopia.Biz;

public class SocialInsuranceRegistrationService : ISocialInsuranceRegistrationService
{
    private readonly ICitizenRepository _citizenRepository;
    private readonly ISocialInsuranceRegistrationRepository _registrationRepository;

    public SocialInsuranceRegistrationService(
        ICitizenRepository citizenRepository,
        ISocialInsuranceRegistrationRepository registrationRepository)
    {
        _citizenRepository = citizenRepository;
        _registrationRepository = registrationRepository;
    }

    public async Task<SocialInsuranceRegistrationDto> CreateForCitizenAsync(
        string nationalId,
        CreateSocialInsuranceRegistrationRequest req)
    {
        if (string.IsNullOrWhiteSpace(nationalId))
            throw new ArgumentException("Missing identity");

        if (string.IsNullOrWhiteSpace(req.SocialInsuranceNumber))
            throw new ArgumentException("SocialInsuranceNumber is required");

        if (string.IsNullOrWhiteSpace(req.SocialInsuranceProvider))
            throw new ArgumentException("SocialInsuranceProvider is required");

        var citizen = await _citizenRepository.GetByNationalIdAsync(nationalId.Trim());
        if (citizen == null)
            throw new ArgumentException("Citizen not found");

        var pending = await _registrationRepository.GetPendingByCitizenIdAsync(citizen.Id);
        if (pending != null)
            throw new InvalidOperationException("Bạn đã có yêu cầu đăng ký đang chờ duyệt.");

        var entity = new SocialInsuranceRegistration
        {
            CitizenId = citizen.Id,
            SocialInsuranceNumber = req.SocialInsuranceNumber.Trim(),
            SocialInsuranceProvider = req.SocialInsuranceProvider.Trim(),
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _registrationRepository.CreateAsync(entity);
        entity.Citizen = citizen;

        return Map(entity);
    }

    public async Task<SocialInsuranceRegistrationDto?> GetLatestForCitizenAsync(string nationalId)
    {
        if (string.IsNullOrWhiteSpace(nationalId))
            return null;

        var citizen = await _citizenRepository.GetByNationalIdAsync(nationalId.Trim());
        if (citizen == null)
            return null;

        var registrations = await _registrationRepository.ListByCitizenIdAsync(citizen.Id);
        var latest = registrations.FirstOrDefault();
        if (latest == null)
            return null;

        latest.Citizen = citizen;
        return Map(latest);
    }

    public async Task<List<SocialInsuranceRegistrationDto>> ListPendingAsync()
    {
        var items = await _registrationRepository.ListPendingAsync();
        return items.Select(Map).ToList();
    }

    public async Task<bool> ReviewAsync(int id, ReviewSocialInsuranceRegistrationRequest req, string reviewer)
    {
        var entity = await _registrationRepository.GetByIdWithCitizenAsync(id);
        if (entity == null)
            return false;

        if (!string.Equals(entity.Status, "Pending", StringComparison.OrdinalIgnoreCase))
            return true;

        if (!string.Equals(req.Status, "Approved", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(req.Status, "Rejected", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Status must be Approved or Rejected");
        }

        entity.Status = req.Status;
        entity.Note = string.IsNullOrWhiteSpace(req.Note) ? null : req.Note.Trim();
        entity.ReviewedAt = DateTime.UtcNow;
        entity.ReviewedBy = string.IsNullOrWhiteSpace(reviewer) ? null : reviewer.Trim();

        if (entity.Status == "Approved" && entity.Citizen != null)
        {
            entity.Citizen.SocialInsuranceNumber = entity.SocialInsuranceNumber;
            entity.Citizen.SocialInsuranceProvider = entity.SocialInsuranceProvider;
        }

        await _registrationRepository.UpdateAsync(entity);
        return true;
    }

    private static SocialInsuranceRegistrationDto Map(SocialInsuranceRegistration entity)
    {
        return new SocialInsuranceRegistrationDto
        {
            Id = entity.Id,
            CitizenId = entity.CitizenId,
            FullName = entity.Citizen?.FullName ?? "",
            NationalId = entity.Citizen?.NationalId ?? "",
            SocialInsuranceNumber = entity.SocialInsuranceNumber,
            SocialInsuranceProvider = entity.SocialInsuranceProvider,
            Status = entity.Status,
            Note = entity.Note,
            CreatedAt = entity.CreatedAt,
            ReviewedAt = entity.ReviewedAt,
            ReviewedBy = entity.ReviewedBy
        };
    }
}