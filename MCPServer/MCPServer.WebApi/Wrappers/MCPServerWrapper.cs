using Azure.AI.OpenAI;
using MCPServer.WebApi.Wrappers.Interfaces;
using ModelContextProtocol.Client;
using OpenAI;
using OpenAI.Chat;
using System.Text.Json;

namespace MCPServer.WebApi.Wrappers
{
    public class MCPServerWrapper : IMCPServerWrapper
    {
        private readonly ILogger<MCPServerWrapper> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly OpenAIClient _openAIClient;

        // MCP Endpoint config
        private readonly string MCPEndpoint = string.Empty;
        private readonly string ClientName = string.Empty;

        public MCPServerWrapper(
            IConfiguration configuration,
            ILogger<MCPServerWrapper> logger,
            OpenAIClient openAIClient)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = new HttpClient();
            MCPEndpoint = _configuration["MCP:ServerEndpoint"];
            ClientName = _configuration["MCP:ClientName"];
            _openAIClient = openAIClient;
        }

        public async Task<string> GetResponseForPrompt(string userMessage)
        {
            // 1. Connect to MCP server
            IMcpClient client = await ConnectWithMCPServer();

            // 2. Get available tools
            IList<McpClientTool> tools = await client.ListToolsAsync();

            // 3. Convert MCP tools into AI function schema
            var aiTools = ChatCompletionFunctionTools(tools);

            // 4. Prepare user message
            var messages = new List<ChatMessage>
            {
                ChatMessage.FromUser(userMessage)
            };

            // 5. Call AI with tool support
            var response = await _openAIClient.Chat.CompleteAsync(
                model: "gpt-4o-mini",
                messages: messages,
                tools: aiTools
            );

            var choice = response.Choices.FirstOrDefault();
            if (choice == null)
                return "No response from AI.";

            var message = choice.Message;

            // 6. Check if AI wants to call a tool
            if (message.ToolCalls != null && message.ToolCalls.Count > 0)
            {
                foreach (var toolCall in message.ToolCalls)
                {
                    string toolName = toolCall.Function.Name;
                    string toolArgs = toolCall.Function.Arguments;

                    // Parse tool arguments (JSON string → dictionary)
                    var args = JsonSerializer.Deserialize<Dictionary<string, object>>(toolArgs);

                    // 7. Call the tool on MCP server
                    var toolResult = await client.CallToolAsync(toolName, args);

                    // 8. Send tool result back to AI so it can finalize response
                    messages.Add(ChatMessage.FromAssistant(message)); // AI request
                    messages.Add(ChatMessage.FromTool(
                        toolCall.Id,
                        JsonSerializer.Serialize(toolResult)
                    ));

                    // Call AI again to get final answer
                    var finalResponse = await _openAIClient.Chat.CompleteAsync(
                        model: "gpt-4o-mini",
                        messages: messages,
                        tools: aiTools
                    );

                    return finalResponse.Content.FirstOrDefault()?.Text
                           ?? "Tool executed but no response.";
                }
            }

            // 9. If no tool call, just return AI response
            return message.Content.FirstOrDefault()?.Text ?? "Empty response.";
        }

        private static List<ChatTool> ChatCompletionFunctionTools(IList<McpClientTool> tools)
        {
            var aiTools = tools.Select(tool => new ChatTool.CreateFunctionTool()

            return aiTools;
        }

        public async IAsyncEnumerable<string> ListTools()
        {
            IMcpClient client = await ConnectWithMCPServer();

            var tools = await client.ListToolsAsync();
            _logger.LogInformation("Listing tools from MCP server");

            foreach (var mcpTools in tools)
            {
                Console.WriteLine($"--- {mcpTools.Name} : {mcpTools.Description}");
                yield return mcpTools.Name;
            }
        }

        private async Task<IMcpClient> ConnectWithMCPServer()
        {
            var sseClientTransport = new SseClientTransport(new()
            {
                Endpoint = new Uri(MCPEndpoint),
                Name = ClientName,
            }, _httpClient);

            return await McpClientFactory.CreateAsync(sseClientTransport);
        }
        private static string GetCurrentLocation()
        {
            // Call the location API here.
            return "San Francisco";
        }

        private static string GetCurrentWeather(string location, string unit = "celsius")
        {
            // Call the weather API here.
            return $"31 {unit}";
        }
        private static readonly ChatTool getCurrentLocationTool = ChatTool.CreateFunctionTool(
                functionName: nameof(GetCurrentLocation),
                functionDescription: "Get the user's current location"
            );

        private static readonly ChatTool getCurrentWeatherTool = ChatTool.CreateFunctionTool(
            functionName: nameof(GetCurrentWeather),
            functionDescription: "Get the current weather in a given location",
            functionParameters: BinaryData.FromBytes("""
        {
            "type": "object",
            "properties": {
                "location": {
                    "type": "string",
                    "description": "The city and state, e.g. Boston, MA"
                },
                "unit": {
                    "type": "string",
                    "enum": [ "celsius", "fahrenheit" ],
                    "description": "The temperature unit to use. Infer this from the specified location."
                }
            },
            "required": [ "location" ]
        }
        """u8.ToArray())
        );
    }
}
