using Microsoft.EntityFrameworkCore;
using Zootopia.Data.Model.Entities;

namespace Zootopia.Data;

public class CitizenRepository : ICitizenRepository
{
    private readonly AppDbContext _context;

    public CitizenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Citizen>> ListAsync(string? q)
    {
        var query = _context.Citizens.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();
            query = query.Where(x =>
                (x.FullName ?? "").Contains(q) ||
                (x.NationalId ?? "").Contains(q));
        }

        return await query
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }

    public async Task<Citizen> CreateAsync(Citizen citizen)
    {
        _context.Citizens.Add(citizen);
        await _context.SaveChangesAsync();
        return citizen;
    }
    public async Task<Citizen?> GetAsync(int id)
    {
        return await _context.Citizens.FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<Citizen?> GetByNationalIdAsync(string nationalId)
    {
        return await _context.Citizens
            .FirstOrDefaultAsync(x => x.NationalId == nationalId);
    }
    public async Task UpdateAsync(Citizen entity)
    {
        _context.Citizens.Update(entity);
        await _context.SaveChangesAsync();
    }

}
