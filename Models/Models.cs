namespace TheAgencyApi.Models;

// Customer, Appointment, 

public class Customer
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public List<Appointment> Appointments { get; set; } = [];
}

public class Appointment
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public required Guid Token { get; set; }
    public Customer? Customer { get; set; }
    public int CustomerId { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
}