using Microsoft.EntityFrameworkCore;
using TheAgencyApi.Data;
using TheAgencyApi.Models;

namespace TheAgencyApi.DAL;

public interface IAppointmentRepository : IBaseRepositoryWrite<Appointment>
{
    Task<List<Appointment>> GetAll();
    Task<List<Appointment>> GetByPeriod(DateTime startDate, DateTime endDate);
    Task<List<Appointment>> GetByDate(DateTime date);
    Task<Appointment?> GetById(int id);
    Task<Appointment?> GetByToken(Guid token);
    Task<int> CountByDate(DateTime date);
    Task<List<AppointmentCount>> CountByPeriod(DateTime startDate, DateTime endDate);
}

public class AppointmentRepository : IAppointmentRepository
{
    private readonly TheAgencyDbContext _context;

    public AppointmentRepository(TheAgencyDbContext context)
    {
        _context = context;
    }

    public async Task<List<Appointment>> GetAll()
    {
        var query = GetAppointmentsWithCustomer();
        return await query.ToListAsync();
    }

    public async Task<List<Appointment>> GetByPeriod(DateTime startDate, DateTime endDate)
    {
        var query = from appointment in GetAppointmentsWithCustomer()
                    where appointment.Date >= startDate && appointment.Date <= endDate
                    select appointment;

        return await query.ToListAsync();
    }


    public async Task<List<Appointment>> GetByDate(DateTime date)
    {
        var query = from appointment in GetAppointmentsWithCustomer()
                    where appointment.Date == date
                    select appointment;

        return await query.ToListAsync();
    }

    public async Task<Appointment?> GetById(int id)
    {
        return await _context.Appointment.FindAsync(id);
    }

    public async Task<Appointment?> GetByToken(Guid token)
    {
        return await _context.Appointment.FirstOrDefaultAsync(x => x.Token == token);
    }

    public async Task<int> CountByDate(DateTime date)
    {
        return await _context.Appointment.CountAsync(x => x.Date == date);
    }

    public async Task<List<AppointmentCount>> CountByPeriod(DateTime startDate, DateTime endDate)
    {
        var query = from appointment in _context.Appointment
                    where appointment.Date >= startDate && appointment.Date <= endDate
                    group appointment by appointment.Date into g
                    select new AppointmentCount
                    {
                        Date = g.Key,
                        Count = g.Count()
                    };

        return await query.ToListAsync();
    }

    public Appointment Create(Appointment appointment)
    {
        _context.Appointment.Add(appointment);
        return appointment;
    }

    public Appointment Update(Appointment appointment)
    {
        _context.Entry(appointment).State = EntityState.Modified;
        return appointment;
    }

    public void Delete(Appointment appointment)
    {
        _context.Appointment.Remove(appointment);
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }

    private IQueryable<Appointment> GetAppointmentsWithCustomer()
    {
        var query = from appointment in _context.Appointment
                    join customer in _context.Customer on appointment.CustomerId equals customer.Id
                    select new Appointment
                    {
                        Id = appointment.Id,
                        Date = appointment.Date,
                        Token = appointment.Token,
                        Customer = new Customer
                        {
                            FullName = customer.FullName
                        },
                        CustomerId = appointment.CustomerId,
                        Location = appointment.Location,
                        Notes = appointment.Notes
                    };

        return query;
    }
}