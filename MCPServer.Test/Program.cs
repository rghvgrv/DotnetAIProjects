using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

var client = new HttpClient();
var uri = "http://localhost:3001";

var transport = new SseClientTransport(new()
{
    Endpoint = new Uri(uri),
    Name = "Secure Client",
}, client);

var clientMCP = await McpClientFactory.CreateAsync(transport);

var tools = await clientMCP.ListToolsAsync(); // For Listing all the tools

var results = await clientMCP.CallToolAsync(
    "echo",
    new Dictionary<string, object?> { { "msg", "World" } }
    );

Console.WriteLine("Results : " + ((TextContentBlock)results.Content[0]).Text);