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

    public AppointmentService(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
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
        // TODO: Validate day is not off and other criteria
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
            CustomerName = appointment.Customer?.FullName ?? "",
            Date = appointment.Date,
            Token = appointment.Token
        };
    }
}


