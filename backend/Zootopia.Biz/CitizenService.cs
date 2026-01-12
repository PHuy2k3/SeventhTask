using Zootopia.Biz.Model.Citizens;
using Zootopia.Biz.Model.Requests;
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

        var entity = new Citizen
        {
            FullName = dto.FullName.Trim(),
            NationalId = dto.NationalId.Trim(),
            DateOfBirth = dto.DateOfBirth,
            AddressText = string.IsNullOrWhiteSpace(dto.AddressText) ? null : dto.AddressText.Trim(),

            // Nếu DB có default/trigger thì bạn có thể bỏ 2 dòng này.
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
            AddressText = entity.AddressText
            // thêm field khác nếu bạn có
        };
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

        await _repo.UpdateAsync(entity);
        return true;
    }
}
