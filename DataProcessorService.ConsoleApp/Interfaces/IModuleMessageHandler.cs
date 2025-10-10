namespace DataProcessorService.ConsoleApp.Interfaces
{
    public interface IModuleMessageHandler
    {
        Task HandleAsync(string message);
    }
}
