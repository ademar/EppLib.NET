using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using EppLib.Entities;

namespace EppLib.Extensions.Nominet.DomainCheck
{
    public class NominetDomainCheckExtension : NominetDirectRightsExtensionBase
    {
        public PostalInfo PostalInfo { get; set; }
        public string Email { get; set; }
        public string Registrant { get; set; }

        public override XmlNode ToXml(XmlDocument doc)
        {
            var root = CreateElement(doc, "nom-direct-rights:check");

            if (PostalInfo != null)
            {
                root.SetAttribute("xmlns:contact", ContactNamespace);

                var xml = AddressToXml(doc, "nom-direct-rights:postalInfo", PostalInfo);

                root.AppendChild(xml);
            }

            if (Email != null)
            {
                AddXmlElement(doc, root, "nom-direct-rights:email", Email);
            }

            if (Registrant != null)
            {
                AddXmlElement(doc, root, "nom-direct-rights:registrant", Registrant);
            }

            return root;
        }

        protected XmlElement AddressToXml(XmlDocument doc,
                                          String tagName,
                                          PostalInfo nameAddress)
        {
            XmlElement name_address_element = null;

            if (nameAddress != null)
            {
                name_address_element = doc.CreateElement(tagName, Namespace);

                if (nameAddress.m_type == null)
                {
                    throw new EppException("missing the address type (postal info)");
                }

                name_address_element.SetAttribute("type", nameAddress.m_type);

                if (nameAddress.m_name != null)
                {
                    AddXmlElement(doc, name_address_element, "contact:name", nameAddress.m_name, ContactNamespace);
                }
                if (nameAddress.m_org != null)
                {
                    AddXmlElement(doc, name_address_element, "contact:org", nameAddress.m_org, ContactNamespace);
                }

                if (nameAddress.m_address != null)
                {
                    var address = nameAddress.m_address;
                    var address_element = doc.CreateElement("contact:addr", ContactNamespace);
                    // Because this method is used by contact create and update,
                    // the lowest common denominator (update), says that all
                    // members are optional.
                    if (address.Street1 != null)
                    {
                        AddXmlElement(doc, address_element, "contact:street", address.Street1, ContactNamespace);
                    }
                    if (address.Street2 != null)
                    {
                        AddXmlElement(doc, address_element, "contact:street", address.Street2, ContactNamespace);
                    }
                    if (address.Street3 != null)
                    {
                        AddXmlElement(doc, address_element, "contact:street", address.Street3, ContactNamespace);
                    }
                    if (address.City != null)
                    {
                        AddXmlElement(doc, address_element, "contact:city", address.City, ContactNamespace);
                    }
                    if (address.StateProvince != null)
                    {
                        AddXmlElement(doc, address_element, "contact:sp", address.StateProvince, ContactNamespace);
                    }
                    if (address.PostalCode != null)
                    {
                        AddXmlElement(doc, address_element, "contact:pc", address.PostalCode, ContactNamespace);
                    }
                    if (address.CountryCode != null)
                    {
                        AddXmlElement(doc, address_element, "contact:cc", address.CountryCode, ContactNamespace);
                    }

                    if (address_element.ChildNodes.Count > 0)
                    {
                        name_address_element.AppendChild(address_element);
                    }
                }
            }
            
            return name_address_element;
        }
    }
}
