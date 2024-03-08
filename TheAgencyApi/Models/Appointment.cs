using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TheAgencyApi.Models;


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

public class AppointmentCount
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
}