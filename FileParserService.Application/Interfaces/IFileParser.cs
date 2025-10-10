using FileParserService.Domain;

namespace FileParserService.Application.Interfaces
{
    public interface IFileParser
    {
        Task<IEnumerable<Module>> ParseAsync(string filePath, CancellationToken cancellationToken);
    }
}
