using Zootopia.Data.Model.Entities;

namespace Zootopia.Data;

public interface ICitizenRepository
{
    Task<List<Citizen>> ListAsync(string? q);
    Task<Citizen?> GetAsync(int id);
    Task<Citizen> CreateAsync(Citizen citizen);
    Task<Citizen?> GetByNationalIdAsync(string nationalId);
    Task UpdateAsync(Citizen entity);
}
