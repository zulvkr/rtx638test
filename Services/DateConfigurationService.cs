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

    public DateConfigurationService(IDateConfigurationRepository dateConfigurationRepository)
    {
        _dcRepository = dateConfigurationRepository;
    }

    public async Task<DateConfigurationDTO> GetByDate(DateTime date)
    {
        var defaultDateConfiguration = await _dcRepository.GetDefault();

        var dateConfiguration = await _dcRepository.GetByDate(date);

        return InternalGetDateConfiguration(date, defaultDateConfiguration, dateConfiguration);
    }

    public async Task<List<DateConfigurationDTO>> GetByPeriod(DateTime startDate, DateTime endDate)
    {
        var defaultDateConfiguration = await _dcRepository.GetDefault();
        var configuredDates = await _dcRepository.GetByPeriod(startDate, endDate);

        var dateConfigurations = new List<DateConfigurationDTO>();
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var dateConfiguration = configuredDates.FirstOrDefault(x => x.Date == date);
            dateConfigurations.Add(
                InternalGetDateConfiguration(date, defaultDateConfiguration, dateConfiguration)
                );
        }

        return dateConfigurations;
    }

    public async Task<List<DateConfigurationDTO>> GetAllConfiguredDates()
    {
        var configuredDates = await _dcRepository.GetAll();
        return configuredDates.Select(x => new DateConfigurationDTO
        {
            Date = x.Date,
            // TODO: This should be the number of appointments for the day
            AppointmentCount = 0,
            MaxAppointments = x.MaxAppointments,
            IsOffDay = x.IsOffDay
        }).ToList();
    }

    private static DateConfigurationDTO InternalGetDateConfiguration(DateTime date, DefaultDateConfiguration defaultDateConfiguration, DateConfiguration? dateConfiguration)
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
                // TODO: This should be the number of appointments for the day
                AppointmentCount = 0,
                MaxAppointments = defaultDateConfiguration.MaxAppointments,
                IsOffDay = isRecurrentHoliday
            };
        }

        return new DateConfigurationDTO
        {
            Date = dateConfiguration.Date,
            // TODO: This should be the number of appointments for the day
            AppointmentCount = 0,
            MaxAppointments = dateConfiguration.MaxAppointments ?? defaultDateConfiguration.MaxAppointments,
            IsOffDay = dateConfiguration.IsOffDay
        };
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

    private DateConfigurationDTO MapToDTO(DateConfiguration dateConfiguration)
    {
        return new DateConfigurationDTO
        {
            Date = dateConfiguration.Date,
            MaxAppointments = dateConfiguration.MaxAppointments,
            IsOffDay = dateConfiguration.IsOffDay
        };
    }

}