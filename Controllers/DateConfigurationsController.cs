using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheAgencyApi.DTO;
using TheAgencyApi.Services;

namespace TheAgencyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DateConfigurationsController : ControllerBase
    {
        private readonly IDateConfigurationService _dcService;

        public DateConfigurationsController(IDateConfigurationService dcService)
        {
            _dcService = dcService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DateConfigurationDTO>>> GetDateConfigurations(
            [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            return await _dcService.GetByPeriod(startDate, endDate);
        }

        [HttpGet("{date}")]
        public async Task<ActionResult<DateConfigurationDTO>> GetDateConfiguration(DateTime date)
        {
            return await _dcService.GetByDate(date);
        }

        [HttpPut("{date}")]
        public async Task<IActionResult> PutDateConfiguration(DateTime date, DateConfigurationDTO dateConfiguration)
        {
            if (date != dateConfiguration.Date)
            {
                return BadRequest();
            }

            try
            {
                await _dcService.Update(dateConfiguration);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _dcService.GetByDate(date) == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<DateConfigurationDTO>> PostDateConfiguration(DateConfigurationDTO dateConfiguration)
        {
            await _dcService.Create(dateConfiguration);

            return CreatedAtAction("GetDateConfiguration", new { date = dateConfiguration.Date }, dateConfiguration);
        }

        [HttpDelete("{date}")]
        public async Task<IActionResult> DeleteDateConfiguration(DateTime date)
        {
            var dateConfiguration = await _dcService.GetByDate(date);
            if (dateConfiguration == null)
            {
                return NotFound();
            }

            await _dcService.Delete(date);

            return NoContent();
        }
    }
}
