using System.Text.Json;

namespace MCPServer.WebApi.Models
{
    public class McpClientTool
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public JsonDocument InputSchema { get; set; }
    }

}
