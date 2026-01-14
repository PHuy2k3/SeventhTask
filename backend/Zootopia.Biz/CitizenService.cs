using Zootopia.Biz.Model.Citizens;
using Zootopia.Biz.Model.Requests;
using Zootopia.Biz.Security;
using Zootopia.Data;
using Zootopia.Data.Model.Entities;

namespace Zootopia.Biz;

public class CitizenService : ICitizenService
{
    private readonly ICitizenRepository _repo;

    public CitizenService(ICitizenRepository repo)
    {
        _repo = repo;
    }

    public Task<List<Citizen>> ListAsync(string? q) => _repo.ListAsync(q);

    public async Task<Citizen> CreateAsync(CreateCitizenRequest dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FullName))
            throw new ArgumentException("FullName is required");

        if (string.IsNullOrWhiteSpace(dto.NationalId))
            throw new ArgumentException("NationalId is required");
        var password = string.IsNullOrWhiteSpace(dto.Password)
         ? dto.NationalId.Trim()
         : dto.Password.Trim();

        var entity = new Citizen
        {
            FullName = dto.FullName.Trim(),
            NationalId = dto.NationalId.Trim(),
            DateOfBirth = dto.DateOfBirth,
            AddressText = string.IsNullOrWhiteSpace(dto.AddressText) ? null : dto.AddressText.Trim(),
            PasswordHash = PasswordHasher.Hash(password),
            IsActive = dto.IsActive ?? true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _repo.CreateAsync(entity);
    }
    public async Task<CitizenDto?> GetByIdAsync(int id)
    {
        var entity = await _repo.GetAsync(id);
        if (entity == null) return null;

        return new CitizenDto
        {
            Id = entity.Id,
            FullName = entity.FullName,
            NationalId = entity.NationalId,
            DateOfBirth = entity.DateOfBirth,
            AddressText = entity.AddressText,
            IsActive = entity.IsActive
            // thêm field khác nếu bạn có
        };
    }
    public async Task<CitizenDto?> GetByNationalIdAsync(string nationalId)
    {
        if (string.IsNullOrWhiteSpace(nationalId)) return null;
        var entity = await _repo.GetByNationalIdAsync(nationalId.Trim());
        if (entity == null) return null;

        return new CitizenDto
        {
            Id = entity.Id,
            FullName = entity.FullName,
            NationalId = entity.NationalId,
            DateOfBirth = entity.DateOfBirth,
            AddressText = entity.AddressText,
            IsActive = entity.IsActive
        };
    }
    public async Task<CitizenDto?> AuthenticateCitizenAsync(string nationalId, string password)
    {
        if (string.IsNullOrWhiteSpace(nationalId) || string.IsNullOrWhiteSpace(password))
            return null;

        var entity = await _repo.GetByNationalIdAsync(nationalId.Trim());
        if (entity == null || !entity.IsActive)
            return null;

        if (string.IsNullOrWhiteSpace(entity.PasswordHash))
        {
            if (!string.Equals(password.Trim(), entity.NationalId, StringComparison.Ordinal))
                return null;

            entity.PasswordHash = PasswordHasher.Hash(password.Trim());
            await _repo.UpdateAsync(entity);
        }
        else if (!PasswordHasher.Verify(password, entity.PasswordHash))
        {
            return null;
        }

        return new CitizenDto
        {
            Id = entity.Id,
            FullName = entity.FullName,
            NationalId = entity.NationalId,
            DateOfBirth = entity.DateOfBirth,
            AddressText = entity.AddressText,
             IsActive = entity.IsActive
        };
    }
    public async Task<bool> ChangePasswordAsync(string nationalId, string currentPassword, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(nationalId) ||
            string.IsNullOrWhiteSpace(currentPassword) ||
            string.IsNullOrWhiteSpace(newPassword))
        {
            return false;
        }

        var entity = await _repo.GetByNationalIdAsync(nationalId.Trim());
        if (entity == null) return false;

        if (string.IsNullOrWhiteSpace(entity.PasswordHash))
        {
            if (!string.Equals(currentPassword.Trim(), entity.NationalId, StringComparison.Ordinal))
                return false;
        }
        else if (!PasswordHasher.Verify(currentPassword, entity.PasswordHash))
        {
            return false;
        }

        entity.PasswordHash = PasswordHasher.Hash(newPassword.Trim());
        await _repo.UpdateAsync(entity);
        return true;
    }
    public async Task<bool> UpdateAsync(int id, UpdateCitizenRequest req)
    {
        var entity = await _repo.GetAsync(id);
        if (entity == null) return false;

        // update fields
        entity.FullName = req.FullName?.Trim();
        entity.NationalId = req.NationalId?.Trim();
        entity.DateOfBirth = req.DateOfBirth;
        entity.AddressText = req.AddressText?.Trim();
        if (req.IsActive.HasValue)
            entity.IsActive = req.IsActive.Value;
        if (!string.IsNullOrWhiteSpace(req.Password))
            entity.PasswordHash = PasswordHasher.Hash(req.Password.Trim());
        await _repo.UpdateAsync(entity);
        return true;
    }
}
