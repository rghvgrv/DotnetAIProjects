using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCPServer
{
    [McpServerToolType]
    public static class EchoTool
    {
        [McpServerTool,Description("TestingTool")]
        public static string Echo(string msg) => $"Hello {msg}";
    }
}
