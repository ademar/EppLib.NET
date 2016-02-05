using System.Xml;
using EppLib.Entities;

namespace EppLib.Extensions.Nominet.Notifications
{
	public class ReferralRejecteNotification : PollResponse
	{
		public string RejectedDomain { get; set; }
		public string RejectedReason { get; set; }

		public ReferralRejecteNotification(string xml) : base(xml) { }
		public ReferralRejecteNotification(byte[] bytes) : base(bytes) { }

		protected override void ProcessDataNode(XmlDocument doc, XmlNamespaceManager namespaces)
		{
			base.ProcessDataNode(doc, namespaces);

			namespaces.AddNamespace("n", "http://www.nominet.org.uk/epp/xml/std-notifications-1.2");
			var failDataNode = doc.SelectSingleNode("/ns:epp/ns:response/ns:resData/n:domainFailData", namespaces);

			if (failDataNode != null)
			{
				var domainName = failDataNode.SelectSingleNode("n:domainName", namespaces);
				if (domainName != null)
				{
					RejectedDomain = domainName.InnerText;
				}

				var reasonNode = failDataNode.SelectSingleNode("n:reason", namespaces);
				if (reasonNode != null)
				{
					RejectedReason = reasonNode.InnerText;
				}
			}
		}
	}
}