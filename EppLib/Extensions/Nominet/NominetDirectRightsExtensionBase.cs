using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EppLib.Entities;

namespace EppLib.Extensions.Nominet
{
    public abstract class NominetDirectRightsExtensionBase : EppExtension
    {
        private string _ns = "http://www.nominet.org.uk/epp/xml/nom-direct-rights-1.0";
        private string _contactNs = "urn:ietf:params:xml:ns:contact-1.0";

        protected override string Namespace
        {
            get { return _ns; }
            set { _ns = value; }
        }

        protected string ContactNamespace
        {
            get { return _contactNs; }
            set { _contactNs = value; }
        }
    }
}
