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
