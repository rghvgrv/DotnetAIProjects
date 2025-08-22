
namespace MCPServer.WebApi.Wrappers.Interfaces
{
    public interface IMCPServerWrapper
    {
        Task<string> GetResponseForPrompt(string userMessage);
        IAsyncEnumerable<string> ListTools();
    }
}
