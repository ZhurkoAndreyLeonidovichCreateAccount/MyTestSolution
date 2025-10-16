using System.Xml.Serialization;

namespace FileParserService.Domain
{
    public class DeviceStatus
    {
        [XmlElement("ModuleCategoryID")]
        public string ModuleCategoryID { get; set; } = string.Empty;

        [XmlElement("RapidControlStatus")]
        public string RapidControlStatus { get; set; } = string.Empty;
      
    }
}
