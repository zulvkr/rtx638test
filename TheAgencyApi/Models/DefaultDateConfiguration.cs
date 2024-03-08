namespace TheAgencyApi.Models;

public class DefaultDateConfiguration
{
    public int Id { get; set; }
    public DayOfWeek[]? WeeklyHolidays
    {
        get;
        set;
    }
    public DateTime[]? YearlyHolidays
    {
        get;
        set;
    }
    public int? MaxAppointments { get; set; }
}
