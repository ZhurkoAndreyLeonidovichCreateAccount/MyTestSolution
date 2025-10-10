using System.Xml.Serialization;

namespace FileParserService.Domain
{
    public class DeviceStatus
    {
        [XmlElement("ModuleCategoryID")]
        public string ModuleCategoryID { get; set; } = string.Empty;
    }
}
