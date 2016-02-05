using System.Collections.Generic;
using System.Xml;
using EppLib.Entities;

namespace EppLib.Extensions.Nominet.Notifications
{
	public class DomainsReleasedNotification : PollResponse
	{
		public string AccountId { get; set; }
		public string From { get; set; }
		public string RegistrarTag { get; set; }
		public List<string> DomainsReleased { get; set; }

		public DomainsReleasedNotification(string xml) : base(xml) { }
		public DomainsReleasedNotification(byte[] bytes) : base(bytes) { }

		protected override void ProcessDataNode(XmlDocument doc, XmlNamespaceManager namespaces)
		{
			base.ProcessDataNode(doc, namespaces);

			namespaces.AddNamespace("n", "http://www.nominet.org.uk/epp/xml/std-notifications-1.2");
			var domainReleasedNode = doc.SelectSingleNode("/ns:epp/ns:response/ns:resData/n:relData", namespaces);

            if (domainReleasedNode != null)
			{
				var accountNode = domainReleasedNode.SelectSingleNode("n:accountId", namespaces);
				if (accountNode != null)
				{
					AccountId = accountNode.InnerText;
				}

				var fromNode = domainReleasedNode.SelectSingleNode("n:from", namespaces);
				if (fromNode != null)
				{
					From = fromNode.InnerText;
				}

				var registrarNode = domainReleasedNode.SelectSingleNode("n:registrarTag", namespaces);
				if (registrarNode != null)
				{
					RegistrarTag = registrarNode.InnerText;
				}

				var domainListNode = domainReleasedNode.SelectSingleNode("n:domainListData", namespaces);
				if (domainListNode != null)
				{
					var nodes = domainListNode.SelectNodes("n:domainName", namespaces);
					if (nodes != null)
					{
						DomainsReleased = new List<string>();
						foreach (XmlNode d in nodes)
						{
							DomainsReleased.Add(d.InnerText);
						}
					}
				}
			}
		}
	}
}
