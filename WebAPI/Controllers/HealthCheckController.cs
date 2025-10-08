using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers
{
    public class HealthCheckController : BaseApiController
    {
        private readonly HealthCheckService _healthCheckService;

        public HealthCheckController(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        [HttpGet("health")]
        [SwaggerOperation(Summary = "Returns application health status", Description = "Checks the current status of the application, including the database connection and other registered components")]
        public async Task<IActionResult> GetHealthDetails()
        {
            var report = await _healthCheckService.CheckHealthAsync();

            var response = new
            {
                Status = report.Status.ToString(),
                Checks = report.Entries.Select(entry => new
                {
                    Name = entry.Key,
                    Status = entry.Value.Status.ToString(),
                    Description = entry.Value.Description,
                    Exception = entry.Value.Exception?.Message,
                    Duration = entry.Value.Duration.ToString(),
                }),
                Duration = report.TotalDuration.ToString()
            };

            return Ok(response);
        }
    }
}
