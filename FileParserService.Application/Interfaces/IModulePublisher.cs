using FileParserService.Domain;

namespace FileParserService.Application.Interfaces
{
    public interface IModulePublisher
    {
        Task PublishAsync(IEnumerable<Module> modules);
    }
}
