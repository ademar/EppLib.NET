using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using EppLib.Entities;

namespace EppLib.Extensions.Nominet.Notifications
{
    public class DomainsSuspendedNotification : PollResponse
    {
        public string SuspendedReason { get; set; }
        public DateTime? CancelDate { get; set; }
        public List<string> SuspendedDomains { get; set; }

        public DomainsSuspendedNotification(string xml) : base(xml){ }
        public DomainsSuspendedNotification(byte[] bytes) : base(bytes){ }

        protected override void ProcessDataNode(XmlDocument doc, XmlNamespaceManager namespaces)
        {
            base.ProcessDataNode(doc, namespaces);

            namespaces.AddNamespace("n", "http://www.nominet.org.uk/epp/xml/std-notifications-1.1");
            var suspDataNode = doc.SelectSingleNode("/ns:epp/ns:response/ns:resData/n:suspData", namespaces);

            if (suspDataNode != null)
            {
                var reasonNode = suspDataNode.SelectSingleNode("n:reason", namespaces);
                SuspendedReason = reasonNode != null ? reasonNode.InnerText : null;
                
                var cancelDateNode = suspDataNode.SelectSingleNode("n:cancelDate", namespaces);
                if (cancelDateNode != null)
                {
                    DateTime parsedDate;
                    if (DateTime.TryParse(cancelDateNode.InnerText, out parsedDate))
                    {
                        CancelDate = parsedDate;
                    }
                }

                var domainListNode = suspDataNode.SelectSingleNode("n:domainListData", namespaces);
                if (domainListNode != null)
                {
                    var nodes = domainListNode.SelectNodes("n:domainName", namespaces);
                    if (nodes != null)
                    {
                        SuspendedDomains = new List<string>();
                        foreach (XmlNode d in nodes)
                        {
                            SuspendedDomains.Add(d.InnerText);
                        }
                    }
                }
            }
        }
    }
}
