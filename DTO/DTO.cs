using System.ComponentModel.DataAnnotations;

namespace TheAgencyApi.DTO;

public class DateConfigurationDTO
{
    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Required]
    public bool IsOffDay { get; set; }

    public int? MaxAppointments { get; set; }

    public int AppointmentCount { get; set; }

    public bool IsFullDay
    {
        get
        {
            return AppointmentCount >= MaxAppointments;
        }
    }
}

public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return value != null && (DateTime)value >= DateTime.Today;
    }
}

public class AppointmentDTO
{
    public int Id { get; set; }

    [Required]
    [FutureDate]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    public Guid Token { get; set; }

    [Required]
    public int CustomerId { get; set; }

    public string? CustomerName { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
}