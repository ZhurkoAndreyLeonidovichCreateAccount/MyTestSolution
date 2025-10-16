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
                        var modelState = ExtractModuleState(device.RapidControlStatus);
                      
                        modules.Add(new Module
                        {
                            ModuleCategoryID = device.ModuleCategoryID,
                            State = GetRandomState(modelState)
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

        private ModuleState GetRandomState(string state)
        {
            var exluded = (ModuleState)Enum.Parse(typeof(ModuleState), state);
            var values = Enum.GetValues(typeof(ModuleState)).Cast<ModuleState>();
            var exludedList = values.Where(x => x != exluded).ToList();
            return exludedList[_random.Next(exludedList.Count)];          
        }
     
        private string? ExtractModuleState(string innerXml)
        {          
            innerXml = innerXml
                .Replace(@"<?xml version=""1.0"" encoding=""utf-16""?>", "")
                .Trim();

            Type? targetType = null;

            if (innerXml.Contains("CombinedSamplerStatus"))
                targetType = typeof(CombinedSamplerStatus);
            else if (innerXml.Contains("CombinedPumpStatus"))
                targetType = typeof(CombinedPumpStatus);
            else if (innerXml.Contains("CombinedOvenStatus"))
                targetType = typeof(CombinedOvenStatus);
            else
                return null;

            var serializer = new XmlSerializer(targetType);
            using var reader = new StringReader(innerXml);
            var obj = serializer.Deserialize(reader);
            
            var prop = targetType.GetProperty("ModuleState");
          
            return prop?.GetValue(obj)?.ToString();
        }

    }
}
