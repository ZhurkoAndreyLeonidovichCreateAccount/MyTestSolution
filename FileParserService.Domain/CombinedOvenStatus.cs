using System.Xml.Serialization;

namespace FileParserService.Domain
{
    public class CombinedOvenStatus
    {
        [XmlElement("ModuleState")]
        public string ModuleState { get; set; } = string.Empty;
    }
}
