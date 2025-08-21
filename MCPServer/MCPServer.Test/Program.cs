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

// 🔹 List all available tools
var tools = await clientMCP.ListToolsAsync();
foreach (var tool in tools)
{
    Console.WriteLine($"Tool: {tool.Name} - {tool.Description}");
}

// 🔹 Call Echo tool
var echoResult = await clientMCP.CallToolAsync(
    "echo", // method name
    new Dictionary<string, object?> { { "msg", "World" } }
);

Console.WriteLine("Echo Result : " + ((TextContentBlock)echoResult.Content[0]).Text);

// 🔹 Call AddNumbers tool
var addResult = await clientMCP.CallToolAsync(
    "add_numbers", // method name in your class
    new Dictionary<string, object?>
    {
        { "a", 5 },
        { "b", 7 }
    }
);

Console.WriteLine("AddNumbers Result : " + ((TextContentBlock)addResult.Content[0]).Text);

Console.WriteLine("--------------------------");
