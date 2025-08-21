using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCPServer
{
    [McpServerToolType]
    public static class EchoTool
    {
        [McpServerTool]
        [Description("Echoes back the provided message.")]
        public static string Echo(string msg) => $"Hello {msg}";

        [McpServerTool]
        [Description("Performs a test response with the provided message.")]
        public static string Test(string msg) => $"[Test] Hello {msg}";

        [McpServerTool]
        [Description("Adds two numbers and returns the result.")]
        public static int AddNumbers(int a, int b) => a + b;
    }
}
