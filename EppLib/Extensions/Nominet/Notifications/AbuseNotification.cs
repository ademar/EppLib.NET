using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using EppLib.Entities;

namespace EppLib.Extensions.Nominet.Notifications
{
    public class AbuseNotification : PollResponse
    {
        public string Key { get; set; }
        public string Activity { get; set; }
        public string Source { get; set; }
        public string HostName { get; set; }
        public string Url { get; set; }
        public DateTime? Date { get; set; }
        public string Ip { get; set; }
        public string Nameserver { get; set; }
        public string DnsAdmin { get; set; }
        public string Target { get; set; }
        public YesNoFlag? WholeDomain { get; set; }


        public AbuseNotification(string xml) : base(xml){ }
        public AbuseNotification(byte[] bytes) : base(bytes){ }

        protected override void ProcessDataNode(XmlDocument doc, XmlNamespaceManager namespaces)
        {
            base.ProcessDataNode(doc, namespaces);

            namespaces.AddNamespace("abuse-feed", "http://www.nominet.org.uk/epp/xml/nom-abuse-feed-1.0");
            var abuseFeedNode = doc.SelectSingleNode("/ns:epp/ns:response/ns:resData/abuse-feed:infData", namespaces);

            if (abuseFeedNode != null)
            {
                var keyNode = abuseFeedNode.SelectSingleNode("abuse-feed:key", namespaces);
                if (keyNode != null)
                {
                    DomainName = keyNode.InnerText;
                    Key = keyNode.InnerText;
                }

                var activityNode = abuseFeedNode.SelectSingleNode("abuse-feed:activity", namespaces);
                Activity = activityNode != null ? activityNode.InnerText : null;
                
                var sourceNode = abuseFeedNode.SelectSingleNode("abuse-feed:source", namespaces);
                Source = sourceNode != null ? sourceNode.InnerText : null;

                var hostnameNode = abuseFeedNode.SelectSingleNode("abuse-feed:hostname", namespaces);
                HostName = hostnameNode != null ? hostnameNode.InnerText : null;

                var urlNode = abuseFeedNode.SelectSingleNode("abuse-feed:url", namespaces);
                Url = urlNode != null ? urlNode.InnerText : null;

                var dateNode = abuseFeedNode.SelectSingleNode("abuse-feed:date", namespaces);
                if (dateNode != null)
                {
                    DateTime parsedDate;
                    if (DateTime.TryParse(dateNode.InnerText, out parsedDate))
                    {
                        Date = parsedDate;
                    }
                }

                var ipNode = abuseFeedNode.SelectSingleNode("abuse-feed:ip", namespaces);
                Ip = ipNode != null ? ipNode.InnerText : null;

                var nameserverNode = abuseFeedNode.SelectSingleNode("abuse-feed:nameserver", namespaces);
                Nameserver = nameserverNode != null ? nameserverNode.InnerText : null;

                var dnsAdminNode = abuseFeedNode.SelectSingleNode("abuse-feed:dnsAdmin", namespaces);
                DnsAdmin = dnsAdminNode != null ? dnsAdminNode.InnerText : null;

                var targetNode = abuseFeedNode.SelectSingleNode("abuse-feed:target", namespaces);
                Target = targetNode != null ? targetNode.InnerText : null;

                var wholeDomainNode = abuseFeedNode.SelectSingleNode("abuse-feed:wholeDomain", namespaces);
                if (wholeDomainNode != null)
                {
                    WholeDomain = (YesNoFlag)Enum.Parse(typeof(YesNoFlag), wholeDomainNode.InnerText);
                }
            }
        }
    }
}
