using FileParserService.Application.Interfaces;
using FileParserService.Domain;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace FileParserService.Infrastructure.Services
{
    public class XmlFileParser : IFileParser
    {
        private static readonly Random _random = new();
        private readonly ILogger<XmlFileParser> _logger;

        public XmlFileParser(ILogger<XmlFileParser> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<Module>> ParseAsync(string folderPath, CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(folderPath)) 
            {
                _logger.LogWarning("Folder {FolderPath} does not exist.", folderPath);
                throw new DirectoryNotFoundException($"Directory not found: {folderPath}");
            }
                
            var xmlFiles = Directory.GetFiles(folderPath, "*.xml");

            if (!xmlFiles.Any())
            {
                _logger.LogWarning("No XML files found.");             
                return Enumerable.Empty<Module>();
            }

            var modules = new ConcurrentBag<Module>();

            await Parallel.ForEachAsync(xmlFiles, cancellationToken, async (xmlFile, ct) =>
            {           
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(InstrumentStatus));
                    using FileStream stream = File.OpenRead(xmlFile);

                    if (serializer.Deserialize(stream) is not InstrumentStatus instrumentStatus)
                        return;

                    foreach (var device in instrumentStatus.DeviceStatusList)
                    {
                        modules.Add(new Module
                        {
                            ModuleCategoryID = device.ModuleCategoryID,
                            State = GetRandomState()
                        });
                    }
                    _logger.LogInformation("Processed:{Path}", Path.GetFileName(xmlFile));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing file {FileName}", xmlFile);
                }
            });
            return modules;
        }

        private ModuleState GetRandomState()
        {
            var values = Enum.GetValues(typeof(ModuleState));           
            return (ModuleState)values.GetValue(_random.Next(values.Length))!;
        }
      
    }
}
