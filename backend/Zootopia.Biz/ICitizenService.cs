using Zootopia.Biz.Model.Citizens;
using Zootopia.Biz.Model.Requests;
using Zootopia.Data.Model.Entities;

namespace Zootopia.Biz;

public interface ICitizenService
{
    Task<List<Citizen>> ListAsync(string? q);
    Task<Citizen> CreateAsync(CreateCitizenRequest dto);
    Task<CitizenDto?> GetByIdAsync(int id);
    Task<bool> UpdateAsync(int id, UpdateCitizenRequest req);
}
