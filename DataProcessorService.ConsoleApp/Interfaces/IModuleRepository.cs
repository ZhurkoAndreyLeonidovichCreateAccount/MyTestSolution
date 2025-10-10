using DataProcessorService.ConsoleApp.Models;

namespace DataProcessorService.ConsoleApp.Interfaces
{
    public interface IModuleRepository
    {
        Task InitializeAsync();
        Task UpsertAsync(Module module);
        Task<IEnumerable<Module>> GetAllAsync();
    }
}
