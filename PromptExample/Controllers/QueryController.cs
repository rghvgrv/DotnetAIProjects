using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromptExample.Data;
using PromptExample.Services;

namespace AiQueryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly OpenAIService _openAIService;

        public QueryController(AppDbContext context, OpenAIService openAIService)
        {
            _context = context;
            _openAIService = openAIService;
        }

        public record QueryRequest(string Question);

        [HttpGet("Get")]
        public IActionResult Get()
        {
            return Ok("API is calling");
        }

        [HttpPost("QueryData")]
        public async Task<IActionResult> QueryData([FromBody] QueryRequest request)
        {
            var cityName = await _openAIService.ExtractCityName(request.Question);

            if (string.IsNullOrWhiteSpace(cityName))
                return BadRequest(new { message = "Could not understand city name." });

            var students = await _context.Students
                .Include(s => s.City)
                .Where(s => s.City.Name.Equals(cityName, StringComparison.OrdinalIgnoreCase))
                .Select(s => new { s.Name, s.Age, City = s.City.Name })
                .ToListAsync();

            return Ok(new { city = cityName, students });
        }

        [HttpPost("GenericQuery")]
        public async Task<IActionResult> GenericQueryData([FromBody] QueryRequest request)
        {
            // Schema Design
            var schemaDescription = @"
                Table: Students (Id, Name, Age, CityId)
                Table: Cities (Id, Name)
                ";

            var sqlQuery = await _openAIService.GenerateSqlQuery(request.Question, schemaDescription);

            if (string.IsNullOrWhiteSpace(sqlQuery)) return BadRequest(new { message = "Couldn't generate SQL query" });

            try
            {
                //var results = await _context.Database.SqlQueryRaw<object>(sqlQuery).ToListAsync();
                return Ok(new { query = sqlQuery });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error executing SQL", error = ex.Message, query = sqlQuery });
            }
        }
    }
}
