using DataProcessorService.ConsoleApp.Interfaces;
using DataProcessorService.ConsoleApp.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DataProcessorService.ConsoleApp.Services
{
    public class ModuleMessageHandler : IModuleMessageHandler
    {
        private readonly IModuleRepository _repository;
        private readonly ILogger<ModuleMessageHandler> _logger;

        public ModuleMessageHandler(IModuleRepository repository, ILogger<ModuleMessageHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task HandleAsync(string message)
        {
            try
            {
                var modules = JsonSerializer.Deserialize<List<Module>>(message);

                if (modules == null || modules.Count == 0)
                {
                    _logger.LogWarning("Received empty or invalid module message.");
                    return;
                }

                foreach (var module in modules)
                {
                    await _repository.UpsertAsync(module);
                    _logger.LogInformation("Saved/updated module {ModuleId} with state {State}.",
                        module.ModuleCategoryID, module.State);
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization error.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing module message.");
            }
        }
    }
}
