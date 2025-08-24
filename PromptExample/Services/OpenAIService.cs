using Npgsql;
using System.Text;
using System.Text.Json;

namespace PromptExample.Services
{
    public class OpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string APIKEY = "";
        private readonly string APIURL = "";
        private readonly string SUPABASE_CONN = "";

        public OpenAIService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
            APIKEY = _configuration.GetRequiredSection("OPENAI:ApiKey").Value!;
            APIURL = _configuration.GetRequiredSection("OPENAI:ApiUrl").Value!;
            SUPABASE_CONN = _configuration.GetConnectionString("DefaultConnection")!;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {APIKEY}");
        }

        public async Task<string> ExtractCityName(string question)
        {
            var prompt = $"Extract only the city name from this question: \"{question}\". Reply with only the city name.";

            var requestBody = new
            {
                model = "gpt-4o-mini", // cheaper & faster
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = prompt }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(APIURL, jsonContent);

            if (!response.IsSuccessStatusCode)
                return "";

            var responseContent = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseContent);
            return jsonDoc.RootElement
                          .GetProperty("choices")[0]
                          .GetProperty("message")
                          .GetProperty("content")
                          .GetString()
                          ?.Trim() ?? "";
        }

        public async Task<string> GenerateSqlQuery(string userprompt, string schemadescription)
        {
            var prompt = $@"
                    You are a SQL generator for EF Core InMemoryDatabase.
                    Schema:
                    {schemadescription}

                    Convert the following request into a valid SQL query for this schema:
                    {userprompt}

                    Only return the SQL query, nothing else.
                    ";

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new
                    {
                        role = "system", content = "You are a helpful assistant."
                    },
                    new
                    {
                        role = "user", content = prompt
                    }
                }
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(APIURL, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                return $"Error : {response.Content} , Status : {response.StatusCode}";
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseContent);

            return jsonDoc.RootElement
                          .GetProperty("choices")[0]
                          .GetProperty("message")
                          .GetProperty("content")
                          .GetString()
                          ?.Trim() ?? "";

        }

        public string ConnectWithSupaBase()
        {
            Console.WriteLine(SUPABASE_CONN);
            using var conn = new NpgsqlConnection(SUPABASE_CONN);
            conn.Open();
            return "Able to Connect to the DB";
        }
    }
}
