using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TheAgencyApi.Models;

public class Customer
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public List<Appointment> Appointments { get; set; } = [];
}

[Index(nameof(Token), IsUnique = true)]
public class Appointment
{
    public int Id { get; set; }

    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    public required Guid Token { get; set; }
    public Customer? Customer { get; set; }
    public int CustomerId { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
}

public class DateConfiguration
{
    [Key]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }
    public int? MaxAppointments { get; set; }
    public bool IsOffDay { get; set; }
}
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
