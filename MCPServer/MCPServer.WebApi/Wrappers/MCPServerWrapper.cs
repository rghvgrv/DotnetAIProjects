using MCPServer.WebApi.Wrappers.Interfaces;
using ModelContextProtocol.Client;

namespace MCPServer.WebApi.Wrappers
{
    public class MCPServerWrapper : IMCPServerWrapper
    {
        private readonly ILogger<MCPServerWrapper> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly SseClientTransport _transport;

        //MCP Endpoint
        private readonly string MCPEndpoint = string.Empty;
        private readonly string ClientName = string.Empty;

        public MCPServerWrapper(IConfiguration configuration, ILogger<MCPServerWrapper> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = new HttpClient();
            MCPEndpoint = _configuration["MCP:ServerEndpoint"];
            ClientName = _configuration["MCP:ClientName"];
        }
        public async IAsyncEnumerable<string> ListTools()
        {
            var sseClientTransport = new SseClientTransport(new()
            {
                Endpoint = new Uri(MCPEndpoint),
                Name = ClientName,
            }, _httpClient);

            var clientMCP = await McpClientFactory.CreateAsync(sseClientTransport);

            var tools = await clientMCP.ListToolsAsync(); // Used to list all the tools
            _logger.LogInformation("List of Tools");

            Console.WriteLine("Available tools for the MCP");

            foreach (var mcpTools in tools)
            {
                Console.WriteLine($"--- {mcpTools.Name} : {mcpTools.Description}");
                yield return mcpTools.Name;
            }
        }
    }
}
