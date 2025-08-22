namespace MCPServer.WebApi.Wrappers.Interfaces
{
    public interface IMCPServerWrapper
    {
        IAsyncEnumerable<string> ListTools();
    }
}
