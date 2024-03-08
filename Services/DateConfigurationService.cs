using Microsoft.EntityFrameworkCore;
using TheAgencyApi.DAL;
using TheAgencyApi.DTO;
using TheAgencyApi.Models;

namespace TheAgencyApi.Services;

public interface IDateConfigurationService : IBaseServiceWrite<DateConfigurationDTO, DateTime>
{
    Task<DateConfigurationDTO> GetByDate(DateTime date);
    Task<List<DateConfigurationDTO>> GetByPeriod(DateTime startDate, DateTime endDate);
    Task<List<DateConfigurationDTO>> GetAllConfiguredDates();
}

public class DateConfigurationService : IDateConfigurationService
{
    private readonly IDateConfigurationRepository _dcRepository;
    private readonly IAppointmentRepository _appointmentRepository;

    public DateConfigurationService(
        IDateConfigurationRepository dateConfigurationRepository,
        IAppointmentRepository appointmentRepository
    )
    {
        _dcRepository = dateConfigurationRepository;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<DateConfigurationDTO> GetByDate(DateTime date)
    {
        var defaultDateConfiguration = await _dcRepository.GetDefault();

        var dateConfiguration = await _dcRepository.GetByDate(date);

        if (dateConfiguration == null)
        {
            var appointmentCount = await _appointmentRepository.CountByDate(date);
            return InternalGetDateConfiguration(date, defaultDateConfiguration,
                dateConfiguration, appointmentCount);
        }

        return InternalGetDateConfiguration(date, defaultDateConfiguration,
            dateConfiguration, dateConfiguration.AppointmentCount);
    }

    public async Task<List<DateConfigurationDTO>> GetByPeriod(DateTime startDate, DateTime endDate)
    {
        var defaultDateConfiguration = await _dcRepository.GetDefault();
        var configuredDates = await _dcRepository.GetByPeriod(startDate, endDate);
        var appointmentCounts = await _appointmentRepository.CountByPeriod(startDate, endDate);

        var result = new List<DateConfigurationDTO>();
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var dateConfiguration = configuredDates.FirstOrDefault(x => x.Date == date);
            var appointmentCount = appointmentCounts.FirstOrDefault(x => x.Date == date)?.Count ?? 0;
            result.Add(
                InternalGetDateConfiguration(date, defaultDateConfiguration,
                    dateConfiguration, appointmentCount)
            );
        }

        return result;
    }

    public async Task<List<DateConfigurationDTO>> GetAllConfiguredDates()
    {
        var configuredDates = await _dcRepository.GetAll();
        return configuredDates.Select(x => new DateConfigurationDTO
        {
            Date = x.Date,
            AppointmentCount = x.AppointmentCount,
            MaxAppointments = x.MaxAppointments,
            IsOffDay = x.IsOffDay
        }).ToList();
    }

    public async Task<DateConfigurationDTO> Update(DateConfigurationDTO dateConfiguration)
    {
        var updated = _dcRepository.Update(new DateConfiguration
        {
            Date = dateConfiguration.Date,
            MaxAppointments = dateConfiguration.MaxAppointments,
            IsOffDay = dateConfiguration.IsOffDay
        });

        try
        {
            await _dcRepository.Save();
            return MapToDTO(updated);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (await _dcRepository.GetByDate(dateConfiguration.Date) == null)
            {
                throw new ArgumentException("Date configuration not found");
            }
            throw;
        }
    }

    public async Task<DateConfigurationDTO> Create(DateConfigurationDTO dateConfiguration)
    {
        var created = _dcRepository.Create(new DateConfiguration
        {
            Date = dateConfiguration.Date,
            MaxAppointments = dateConfiguration.MaxAppointments,
            IsOffDay = dateConfiguration.IsOffDay
        });

        await _dcRepository.Save();
        return MapToDTO(created);
    }

    public async Task Delete(DateTime date)
    {
        var dateConfiguration = await _dcRepository.GetByDate(date);
        if (dateConfiguration != null)
        {
            _dcRepository.Delete(dateConfiguration);
            await _dcRepository.Save();
        }
        else
        {
            throw new ArgumentException("Date configuration not found");
        }
    }

    private static DateConfigurationDTO InternalGetDateConfiguration(
        DateTime date,
        DefaultDateConfiguration defaultDateConfiguration,
        DateConfiguration? dateConfiguration,
        int appointmentCount
    )
    {
        var isWeeklyHoliday = defaultDateConfiguration.WeeklyHolidays?
            .Contains(date.DayOfWeek) ?? false;
        var isYearlyHoliday = defaultDateConfiguration.YearlyHolidays?
            .Any(x => x.Month == date.Month && x.Day == date.Day) ?? false;
        var isRecurrentHoliday = isWeeklyHoliday || isYearlyHoliday;

        if (dateConfiguration == null)
        {
            return new DateConfigurationDTO
            {
                Date = date,
                AppointmentCount = appointmentCount,
                MaxAppointments = defaultDateConfiguration.MaxAppointments,
                IsOffDay = isRecurrentHoliday
            };
        }

        return new DateConfigurationDTO
        {
            Date = dateConfiguration.Date,
            AppointmentCount = appointmentCount,
            MaxAppointments = dateConfiguration.MaxAppointments ?? defaultDateConfiguration.MaxAppointments,
            IsOffDay = dateConfiguration.IsOffDay
        };
    }

    private static DateConfigurationDTO MapToDTO(DateConfiguration dateConfiguration)
    {
        return new DateConfigurationDTO
        {
            Date = dateConfiguration.Date,
            MaxAppointments = dateConfiguration.MaxAppointments,
            IsOffDay = dateConfiguration.IsOffDay
        };
    }

}