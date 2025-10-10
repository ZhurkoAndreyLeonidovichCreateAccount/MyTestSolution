using System.Xml.Serialization;

namespace FileParserService.Domain
{
    [XmlRoot("InstrumentStatus")]
    public class InstrumentStatus
    {
        [XmlElement("DeviceStatus")]
        public List<DeviceStatus> DeviceStatusList { get; set; } = new();
    }
}
