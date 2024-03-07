using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TheAgencyApi.DTO;
using TheAgencyApi.Services;

namespace TheAgencyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetAppointment(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return await _appointmentService.GetByPeriod(startDate.Value, endDate.Value);
            }
            else
            {
                return await _appointmentService.GetAll();
            }
        }

        [HttpGet("{date}")]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetAppointment(DateTime date)
        {
            var appointment = await _appointmentService.GetByDate(date);

            if (appointment == null)
            {
                return NotFound();
            }

            return appointment;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointment(int id, AppointmentDTO appointment)
        {
            try
            {
                await _appointmentService.Update(appointment);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<AppointmentDTO>> PostAppointment(AppointmentDTO appointment)
        {
            var created = await _appointmentService.Create(appointment);
            return CreatedAtAction("GetAppointment", new { id = created.Id }, created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            await _appointmentService.Delete(id);
            return NoContent();
        }
    }
}
