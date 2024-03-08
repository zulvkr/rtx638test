using System.ComponentModel.DataAnnotations;

namespace TheAgencyApi.Models;

public class DateConfiguration
{
    [Key]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }
    public int? MaxAppointments { get; set; }
    public bool IsOffDay { get; set; }
}

public class DateConfigurationWithAppointments : DateConfiguration
{
    public int AppointmentCount { get; set; }
}

