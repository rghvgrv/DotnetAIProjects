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
                    You are a PgSQL query generator for EF Core InMemoryDatabase.
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

        public async Task<List<Dictionary<string, object>>> ConnectWithSupaBaseAsync(string sqlQuery)
        {
            try
            {
                await using var conn = new NpgsqlConnection(SUPABASE_CONN);
                await conn.OpenAsync();
                Console.WriteLine("✅ Connected to Supabase successfully.");

                sqlQuery = CleanSqlQuery(sqlQuery);

                await using var cmd = new NpgsqlCommand(sqlQuery, conn);
                await using var reader = await cmd.ExecuteReaderAsync();

                var results = new List<Dictionary<string, object>>();
                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    }
                    results.Add(row);
                }

                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to connect with Supabase: {ex.Message}");
                throw; // preserves stack trace
            }
        }
        private string CleanSqlQuery(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql)) return string.Empty;

            // Remove ```sql and ``` fences
            sql = sql.Replace("```sql", "", StringComparison.OrdinalIgnoreCase)
                     .Replace("```", "");

            return sql.Trim();
        }
    }
}
