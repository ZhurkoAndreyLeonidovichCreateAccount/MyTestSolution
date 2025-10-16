using System.Xml.Serialization;

namespace FileParserService.Domain
{
    public class CombinedSamplerStatus
    {
        [XmlElement("ModuleState")]
        public string ModuleState { get; set; } = string.Empty;
    }
}
