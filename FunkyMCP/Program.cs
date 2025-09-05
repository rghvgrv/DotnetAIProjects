using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole(consoleLogOptions =>
{
    // Configure all logs to go to stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();
await builder.Build().RunAsync();

[McpServerToolType]
public static class EchoTool
{
    [McpServerTool, Description("Echoes the message back to the client.")]
    public static string Echo(string message) => $"hello {message}";

    [McpServerTool, Description("Count the length of characters in the message.")]
    public static int CountCharacters(string message) => message.Length;

    [McpServerTool, Description("Count the number of words in the message.")]
    public static int CountWords(string message) => message.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

    [McpServerTool, Description("Reverses the message.")]
    public static string Reverse(string message) => new string(message.Reverse().ToArray());
}