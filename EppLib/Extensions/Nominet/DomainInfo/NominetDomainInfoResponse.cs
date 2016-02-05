using System.Collections.Generic;
using System.Xml;
using EppLib.Entities;

namespace EppLib.Extensions.Nominet.DomainInfo
{
	public class NominetDomainInfoResponse : DomainInfoResponse
	{
		private NominetDomainExtension _nomDomain = new NominetDomainExtension();
		public virtual new NominetDomainExtension Domain
		{
			get { return _nomDomain; }
		}

		public NominetDomainInfoResponse(byte[] bytes) : base(bytes) { }

		protected override void ProcessDataNode(XmlDocument doc, XmlNamespaceManager namespaces)
		{
			base.ProcessDataNode(doc, namespaces);

			_nomDomain.ClId = _domain.ClId;
			_nomDomain.Contacts = _domain.Contacts;
			_nomDomain.CrDate = _domain.CrDate;
			_nomDomain.CrId = _domain.CrId;
			_nomDomain.ExDate = _domain.ExDate;
			_nomDomain.Hosts = _domain.Hosts;
			_nomDomain.Name = _domain.Name;
			_nomDomain.NameServers = _domain.NameServers;
			_nomDomain.Password = _domain.Password;
			_nomDomain.RegistrantId = _domain.RegistrantId;
			_nomDomain.Roid = _domain.Roid;
			_nomDomain.Status = _domain.Status;
			_nomDomain.TrDate = _domain.TrDate;
			_nomDomain.UpDate = _domain.UpDate;
			_nomDomain.UpId = _domain.UpId;
		}

		protected override void ProcessExtensionNode(XmlDocument doc, XmlNamespaceManager namespaces)
		{
			namespaces.AddNamespace("domain", "urn:ietf:params:xml:ns:domain-1.0");
			namespaces.AddNamespace("domain-nom-ext", "http://www.nominet.org.uk/epp/xml/domain-nom-ext-1.2");

			var children = doc.SelectSingleNode("/ns:epp/ns:response/ns:extension/domain-nom-ext:infData", namespaces);
			
			if (children != null)
			{
				var autoBillNode = children.SelectSingleNode("domain-nom-ext:auto-bill", namespaces);

				if (autoBillNode != null)
				{
					_nomDomain.AutoBill = autoBillNode.InnerText;
				}

				var firstBillNode = children.SelectSingleNode("domain-nom-ext:first-bill", namespaces);

				if (firstBillNode != null)
				{
					_nomDomain.FirstBill = firstBillNode.InnerText;
				}

				var recurBillNode = children.SelectSingleNode("domain-nom-ext:recur-bill", namespaces);

				if (recurBillNode != null)
				{
					_nomDomain.RecurBill = recurBillNode.InnerText;
				}

				var nextBillNode = children.SelectSingleNode("domain-nom-ext:next-bill", namespaces);

				if (nextBillNode != null)
				{
					_nomDomain.FirstBill = nextBillNode.InnerText;
				}

				var resellerNode = children.SelectSingleNode("domain-nom-ext:reseller", namespaces);

				if (resellerNode != null)
				{
					_nomDomain.Reseller = resellerNode.InnerText;
				}

				var nextPeriodNode = children.SelectSingleNode("domain-nom-ext:next-period", namespaces);

				if (nextPeriodNode != null)
				{
					_nomDomain.NextPeriod = nextPeriodNode.InnerText;
				}

				var autoPeriodNode = children.SelectSingleNode("domain-nom-ext:auto-period", namespaces);

				if (autoPeriodNode != null)
				{
					_nomDomain.AutoPeriod = autoPeriodNode.InnerText;
				}

				var renewNotReqNode = children.SelectSingleNode("domain-nom-ext:renew-not-required", namespaces);

				if (renewNotReqNode != null)
				{
					_nomDomain.RenewNotRequired = renewNotReqNode.InnerText;
				}

				var regStatusNode = children.SelectSingleNode("domain-nom-ext:reg-status", namespaces);

				if (regStatusNode != null)
				{
					_nomDomain.RegStatus = regStatusNode.InnerText;
				}

				var noteNodes = children.SelectNodes("domain-nom-ext:notes", namespaces);

				if (noteNodes != null)
				{
					_nomDomain.Notes = new List<string>();
					foreach (XmlNode note in noteNodes)
					{
						_nomDomain.Notes.Add(note.InnerText);
					}
				}
			}
		}
	}
}