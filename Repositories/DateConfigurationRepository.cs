using Microsoft.EntityFrameworkCore;
using TheAgencyApi.Data;
using TheAgencyApi.Models;

namespace TheAgencyApi.DAL;

public interface IDateConfigurationRepository : IBaseRepositoryWrite<DateConfiguration>
{
    Task<DefaultDateConfiguration> GetDefault();
    Task<DateConfigurationWithAppointments?> GetByDate(DateTime date);
    Task<List<DateConfigurationWithAppointments>> GetAll();
    Task<List<DateConfiguration>> GetByPeriod(DateTime startDate, DateTime endDate);
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

    public async Task<DateConfigurationWithAppointments?> GetByDate(DateTime date)
    {
        var query = from dc in _context.DateConfiguration
                    where dc.Date == date
                    join a in _context.Appointment on dc.Date equals a.Date into appointments
                    select new DateConfigurationWithAppointments
                    {
                        Date = dc.Date,
                        IsOffDay = dc.IsOffDay,
                        MaxAppointments = dc.MaxAppointments,
                        AppointmentCount = appointments.Count()
                    };

        return await query.FirstOrDefaultAsync();
    }

    public async Task<List<DateConfigurationWithAppointments>> GetAll()
    {
        var query = from dc in _context.DateConfiguration
                    join a in _context.Appointment on dc.Date equals a.Date into appointments
                    select new DateConfigurationWithAppointments
                    {
                        Date = dc.Date,
                        IsOffDay = dc.IsOffDay,
                        MaxAppointments = dc.MaxAppointments,
                        AppointmentCount = appointments.Count()
                    };

        return await query.ToListAsync();
    }

    public async Task<List<DateConfiguration>> GetByPeriod(DateTime startDate, DateTime endDate)
    {

        var query = from dc in _context.DateConfiguration
                    where dc.Date >= startDate && dc.Date <= endDate
                    select dc;
        return await query.ToListAsync();
    }

    public DateConfiguration Create(DateConfiguration dateConfiguration)
    {
        _context.DateConfiguration.Add(dateConfiguration);
        return dateConfiguration;
    }

    public DateConfiguration Update(DateConfiguration dateConfiguration)
    {
        _context.Entry(dateConfiguration).State = EntityState.Modified;
        return dateConfiguration;
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
