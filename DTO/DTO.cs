namespace TheAgencyApi.DTO;

public class DateConfigurationDTO
{
    public required DateTime Date { get; set; }
    public int AppointmentCount { get; set; }
    public int? MaxAppointments { get; set; }
    public required bool IsOffDay { get; set; }
    public bool IsFullDay
    {
        get
        {
            return AppointmentCount >= MaxAppointments;
        }
    }
}

public class AppointmentDTO
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public Guid Token { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
}