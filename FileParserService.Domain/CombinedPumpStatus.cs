using System.Xml.Serialization;

namespace FileParserService.Domain
{
    public class CombinedPumpStatus
    {
        [XmlElement("ModuleState")]
        public string ModuleState { get; set; } = string.Empty;
    }
}
