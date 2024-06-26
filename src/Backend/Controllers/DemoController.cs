using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class DemoController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<DemoController> _logger;

    public DemoController(ILogger<DemoController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get random weather forecast for authorized users
    /// </summary>
    /// <returns></returns>
    [HttpGet("weather-forecast")]
    [Authorize]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    /// <summary>
    /// Get characters length for authorized users
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("data-processing-users")]
    [Authorize]
    public IActionResult ProcessData1([FromBody] FormModel model)
    {
        return Content($"{model.Message.Length} characters");
    }

    /// <summary>
    /// Get characters length for authorized role is Manager
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("data-processing-managers")]
    [Authorize(Roles = "Manager")]
    public IActionResult ProcessData2([FromBody] FormModel model)
    {
        return Content($"{model.Message.Length} characters");
    }
}
