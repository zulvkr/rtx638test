using Microsoft.EntityFrameworkCore;
using TheAgencyApi.DAL;
using TheAgencyApi.DTO;
using TheAgencyApi.Models;

namespace TheAgencyApi.Services;

public interface IDateConfigurationService
{
    Task<DateConfigurationDTO> GetByDate(DateTime date);
    Task<List<DateConfigurationDTO>> GetByPeriod(DateTime startDate, DateTime endDate);
    Task Update(DateConfigurationDTO dateConfiguration);
    Task Create(DateConfigurationDTO dateConfiguration);
    Task Delete(DateTime date);
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

    private static DateConfigurationDTO InternalGetDateConfiguration(DateTime date, DefaultDateConfiguration defaultDateConfiguration, DateConfiguration? dateConfiguration)
    {
        var isWeeklyHoliday = defaultDateConfiguration.WeeklyHolidays?
            .Contains(date.DayOfWeek) ?? false;
        var isYearlyHoliday = defaultDateConfiguration.YearlyHolidays?
            .Contains(date) ?? false;
        var isRecurrentHoliday = isWeeklyHoliday || isYearlyHoliday;

        if (dateConfiguration == null)
        {
            return new DateConfigurationDTO
            {
                Date = date,
                AppointmentCount = 0,
                MaxAppointments = defaultDateConfiguration.MaxAppointments,
                IsOffDay = isRecurrentHoliday
            };
        }

        return new DateConfigurationDTO
        {
            Date = dateConfiguration.Date,
            AppointmentCount = 0,
            MaxAppointments = dateConfiguration.MaxAppointments ?? defaultDateConfiguration.MaxAppointments,
            IsOffDay = dateConfiguration.IsOffDay || isRecurrentHoliday
        };
    }

    public async Task Update(DateConfigurationDTO dateConfiguration)
    {
        _dcRepository.Update(new DateConfiguration
        {
            Date = dateConfiguration.Date,
            MaxAppointments = dateConfiguration.MaxAppointments,
            IsOffDay = dateConfiguration.IsOffDay
        });

        await _dcRepository.Save();
    }

    public async Task Create(DateConfigurationDTO dateConfiguration)
    {
        _dcRepository.Create(new DateConfiguration
        {
            Date = dateConfiguration.Date,
            MaxAppointments = dateConfiguration.MaxAppointments,
            IsOffDay = dateConfiguration.IsOffDay
        });

        await _dcRepository.Save();
    }

    public async Task Delete(DateTime date)
    {
        var dateConfiguration = await _dcRepository.GetByDate(date);
        if (dateConfiguration != null)
        {
            _dcRepository.Delete(dateConfiguration);
        }
        await _dcRepository.Save();
    }

}