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