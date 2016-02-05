using System.Xml;

namespace EppLib.Extensions.Nominet.ContactUpdate
{
	public class NominetContactUpdateExtension : NominetContactExtensionBase
    {
		public string TradeName { get; set; }
		public CoType? Type { get; set; }
		public string CompanyNumber { get; set; }
		public YesNoFlag? OptOut { get; set; }

		public override XmlNode ToXml(XmlDocument doc)
        {
			var root = CreateElement(doc, "contact-nom-ext:update");

			if (!string.IsNullOrWhiteSpace(TradeName))
			{
				AddXmlElement(doc, root, "contact-nom-ext:trad-name", TradeName);
			}

			if (Type.HasValue)
			{
				AddXmlElement(doc, root, "contact-nom-ext:type", Type.ToString());
			}

			if (!string.IsNullOrWhiteSpace(CompanyNumber))
			{
				AddXmlElement(doc, root, "contact-nom-ext:co-no", CompanyNumber);
			}

			if (OptOut.HasValue)
			{
				AddXmlElement(doc, root, "contact-nom-ext:opt-out", OptOut.ToString());
			}

			return root;
        }
    }
}