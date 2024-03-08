using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TheAgencyApi.DTO;

namespace TheAgencyApi.UnitTests;

public class AppointmentsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{

    readonly HttpClient _client;

    public AppointmentsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_appointment_all()
    {
        var response = await _client.GetAsync("/appointments");
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<List<AppointmentDTO>>(body);
        Assert.Equal(9, json?.Count);
    }

    [Fact]
    public async Task GET_appointment_by_date()
    {
        var response = await _client.GetAsync("/appointments/2030-01-04");
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<List<AppointmentDTO>>(body);
        Assert.Equal(8, json?.Count);
    }
}