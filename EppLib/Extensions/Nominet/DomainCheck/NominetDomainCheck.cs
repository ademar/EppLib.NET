using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using EppLib.Entities;

namespace EppLib.Extensions.Nominet.DomainCheck
{
    public class NominetDomainCheck : Entities.DomainCheck
    {
        public PostalInfo PostalInfo { get; set; }
        public string Email { get; set; }
        public string Registrant { get; set; }

        public NominetDomainCheck(string domain) : base(domain)
        {
        }

        public NominetDomainCheck(IList<string> domains) : base(domains)
        {
        }

        public override XmlDocument ToXml()
        {
            var nominetExtension = new NominetDomainCheckExtension
            {
                Email = Email,
                PostalInfo = PostalInfo,
                Registrant = Registrant
            };
            Extensions.Clear();
            Extensions.Add(nominetExtension);
            return base.ToXml();
        }
    }
}
