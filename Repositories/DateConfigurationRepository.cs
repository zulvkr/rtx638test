using Microsoft.EntityFrameworkCore;
using TheAgencyApi.Data;
using TheAgencyApi.Models;

namespace TheAgencyApi.DAL;

public interface IDateConfigurationRepository
{
    Task<DefaultDateConfiguration> GetDefault();
    Task<DateConfiguration?> GetByDate(DateTime date);
    Task<List<DateConfiguration>> GetAll();
    Task<List<DateConfiguration>> GetByPeriod(DateTime startDate, DateTime endDate);
    void Create(DateConfiguration dateConfiguration);
    void Update(DateConfiguration dateConfiguration);
    void Delete(DateConfiguration dateConfiguration);
    Task Save();

}

public class DateConfigurationRepository : IDateConfigurationRepository
{
    private readonly TheAgencyDbContext _context;

    public DateConfigurationRepository(TheAgencyDbContext context)
    {
        _context = context;
    }


    public async Task<DefaultDateConfiguration> GetDefault()
    {
        return await _context.DefaultDateConfiguration.FirstOrDefaultAsync()
            ?? throw new Exception("Default date configuration not found");
    }

    public async Task<DateConfiguration?> GetByDate(DateTime date)
    {
        return await _context.DateConfiguration.FirstOrDefaultAsync(x => x.Date == date);
    }

    public async Task<List<DateConfiguration>> GetAll()
    {
        return await _context.DateConfiguration.ToListAsync();
    }

    public async Task<List<DateConfiguration>> GetByPeriod(DateTime startDate, DateTime endDate)
    {
        var query = from dateConfiguration in _context.DateConfiguration
                    where dateConfiguration.Date >= startDate && dateConfiguration.Date <= endDate
                    select dateConfiguration;

        return await query.ToListAsync();
    }

    public void Create(DateConfiguration dateConfiguration)
    {
        _context.DateConfiguration.Add(dateConfiguration);
    }

    public void Update(DateConfiguration dateConfiguration)
    {
        _context.Entry(dateConfiguration).State = EntityState.Modified;
    }

    public void Delete(DateConfiguration dateConfiguration)
    {
        _context.DateConfiguration.Remove(dateConfiguration);
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }
}