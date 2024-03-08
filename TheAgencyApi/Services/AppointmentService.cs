using TheAgencyApi.DAL;
using TheAgencyApi.DTO;
using TheAgencyApi.Models;

namespace TheAgencyApi.Services;

public interface IAppointmentService : IBaseServiceWrite<AppointmentDTO, int>
{
    Task<List<AppointmentDTO>> GetAll();
    Task<List<AppointmentDTO>> GetByPeriod(DateTime startDate, DateTime endDate);
    Task<List<AppointmentDTO>> GetByDate(DateTime date);
    Task<AppointmentDTO?> GetById(int id);
    Task<AppointmentDTO?> GetByToken(Guid token);
}

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDateConfigurationService _dateConfigurationService;

    public AppointmentService(IAppointmentRepository appointmentRepository, IDateConfigurationService dateConfigurationService)
    {
        _appointmentRepository = appointmentRepository;
        _dateConfigurationService = dateConfigurationService;
    }

    public async Task<List<AppointmentDTO>> GetAll()
    {
        var appointments = await _appointmentRepository.GetAll();
        return appointments.Select(MapToDTO).ToList();
    }

    public async Task<List<AppointmentDTO>> GetByPeriod(DateTime startDate, DateTime endDate)
    {
        var appointments = await _appointmentRepository.GetByPeriod(startDate, endDate);
        return appointments.Select(MapToDTO).ToList();
    }

    public async Task<List<AppointmentDTO>> GetByDate(DateTime date)
    {
        var appointments = await _appointmentRepository.GetByDate(date);
        return appointments.Select(MapToDTO).ToList();
    }

    public async Task<AppointmentDTO?> GetById(int id)
    {
        var appointment = await _appointmentRepository.GetById(id);
        return appointment == null ? null : MapToDTO(appointment);
    }

    public async Task<AppointmentDTO?> GetByToken(Guid token)
    {
        var appointment = await _appointmentRepository.GetByToken(token);
        return appointment == null ? null : MapToDTO(appointment);
    }

    public async Task<AppointmentDTO> Create(AppointmentDTO appointment)
    {
        // Validation Rules:
        // 1. Day is not off
        // 2. Day is not full
        // 3. If full, check next non off day
        // 4. If next non off day is full, throw exception

        var dateConfiguration = await _dateConfigurationService.GetByDate(appointment.Date);
        if (dateConfiguration.IsOffDay)
        {
            throw new ArgumentException("Day is off");
        }

        if (dateConfiguration.IsFullDay)
        {
            DateConfigurationDTO nextNonOffDateConfiguration;
            do
            {
                appointment.Date = appointment.Date.AddDays(1);
                nextNonOffDateConfiguration = await _dateConfigurationService.GetByDate(appointment.Date);
            } while (nextNonOffDateConfiguration.IsOffDay);

            if (nextNonOffDateConfiguration.IsFullDay)
            {
                throw new ArgumentException($"Selected day and next day are full");
            }
        }

        var created = _appointmentRepository.Create(new Appointment
        {
            CustomerId = appointment.CustomerId,
            Date = appointment.Date,
            Token = Guid.NewGuid()
        });
        await _appointmentRepository.Save();
        return MapToDTO(created);
    }

    public Task<AppointmentDTO> Update(AppointmentDTO appointment)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(int id)
    {
        var appointment = await _appointmentRepository.GetById(id);
        if (appointment != null)
        {
            _appointmentRepository.Delete(appointment);
            await _appointmentRepository.Save();
        }
        else throw new ArgumentException("Appointment not found");
    }

    private AppointmentDTO MapToDTO(Appointment appointment)
    {
        return new AppointmentDTO
        {
            Id = appointment.Id,
            CustomerId = appointment.CustomerId,
            CustomerName = appointment.Customer?.FullName,
            Date = appointment.Date,
            Token = appointment.Token
        };
    }
}


