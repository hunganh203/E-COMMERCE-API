using System.Xml.Serialization;

namespace Application.Dtos
{
    public class CurrencyExchangeRateDto
    {
        public string CurrencyCode { get; set; } = string.Empty;

        public string CurrencyName { get; set; } = string.Empty;

        public double Buy { get; set; }

        public double Transfer { get; set; }

        public double Sell { get; set; }
    }

    public class Exrate
    {
        [XmlAttribute(AttributeName = "CurrencyCode")]
        public string CurrencyCode { get; set; } = string.Empty;

        [XmlAttribute(AttributeName = "CurrencyName")]
        public string CurrencyName { get; set; } = string.Empty;

        [XmlAttribute(AttributeName = "Buy")]
        public string Buy { get; set; } = string.Empty;

        [XmlAttribute(AttributeName = "Transfer")]
        public string Transfer { get; set; } = string.Empty;

        [XmlAttribute(AttributeName = "Sell")]
        public string Sell { get; set; } = string.Empty;
    }

    [XmlRoot(ElementName = "ExrateList")]
    public class ExrateList
    {
        [XmlElement(ElementName = "DateTime")]
        public string DateTime { get; set; } = string.Empty;

        [XmlElement(ElementName = "Exrate")] public List<Exrate> Exrates { get; set; } = new();
        [XmlElement(ElementName = "Source")] public string Source { get; set; } = string.Empty;
    }
}