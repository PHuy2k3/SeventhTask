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
}
