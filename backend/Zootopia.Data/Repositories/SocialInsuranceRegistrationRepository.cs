using Microsoft.EntityFrameworkCore;
using Zootopia.Data.Model.Entities;

using Microsoft.EntityFrameworkCore;
using Zootopia.Data.Model.Entities;

namespace Zootopia.Data;

public class SocialInsuranceRegistrationRepository : ISocialInsuranceRegistrationRepository
{
    private readonly AppDbContext _context;

    public SocialInsuranceRegistrationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SocialInsuranceRegistration> CreateAsync(SocialInsuranceRegistration registration)
    {
        _context.SocialInsuranceRegistrations.Add(registration);
        await _context.SaveChangesAsync();
        return registration;
    }

    public async Task<SocialInsuranceRegistration?> GetByIdAsync(int id)
    {
        return await _context.SocialInsuranceRegistrations.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<SocialInsuranceRegistration?> GetByIdWithCitizenAsync(int id)
    {
        return await _context.SocialInsuranceRegistrations
            .Include(x => x.Citizen)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<SocialInsuranceRegistration?> GetPendingByCitizenIdAsync(int citizenId)
    {
        return await _context.SocialInsuranceRegistrations
            .Where(x => x.CitizenId == citizenId && x.Status == "Pending")
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<List<SocialInsuranceRegistration>> ListPendingAsync()
    {
        return await _context.SocialInsuranceRegistrations
            .Include(x => x.Citizen)
            .Where(x => x.Status == "Pending")
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<SocialInsuranceRegistration>> ListByCitizenIdAsync(int citizenId)
    {
        return await _context.SocialInsuranceRegistrations
            .Where(x => x.CitizenId == citizenId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(SocialInsuranceRegistration registration)
    {
        _context.SocialInsuranceRegistrations.Update(registration);
        await _context.SaveChangesAsync();
    }
}