using System.Collections.Generic;
using System.Xml;
using EppLib.Entities;

namespace EppLib.Extensions.Nominet.Notifications
{
	public class HandshakeRejectedNotification : PollResponse
	{
		public string AccountId { get; set; }
		public string From { get; set; }
		public string RegistrarTag { get; set; }
		public List<string> DomainList { get; set; }

		public HandshakeRejectedNotification(string xml) : base(xml) { }
		public HandshakeRejectedNotification(byte[] bytes) : base(bytes) { }

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
						DomainList = new List<string>();
						foreach (XmlNode d in nodes)
						{
							DomainList.Add(d.InnerText);
						}
					}
				}
			}
		}
	}
}
