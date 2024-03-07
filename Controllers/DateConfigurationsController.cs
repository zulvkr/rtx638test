using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> PutDateConfiguration(DateConfigurationDTO dateConfiguration)
        {
            try
            {
                await _dcService.Update(dateConfiguration);
            }
            catch (ArgumentException)
            {
                return NotFound();
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
            try
            {
                await _dcService.Delete(date);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
