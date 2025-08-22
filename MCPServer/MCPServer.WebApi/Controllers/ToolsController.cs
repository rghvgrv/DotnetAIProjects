using MCPServer.WebApi.Wrappers.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MCPServer.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToolsController : ControllerBase
    {
        private readonly ILogger<ToolsController> _logger;
        private readonly IMCPServerWrapper _mcpServerWrapper;
        public ToolsController(ILogger<ToolsController> logger, IMCPServerWrapper mCPServerWrapper)
        {
            _logger = logger;
            _mcpServerWrapper = mCPServerWrapper;
        }

        [HttpGet("tools")]
        public IAsyncEnumerable<string> GetTools()
        {
            return _mcpServerWrapper.ListTools();
        }
    }
}
