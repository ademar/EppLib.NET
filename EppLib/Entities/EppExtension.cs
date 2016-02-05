// Copyright 2012 Code Maker Inc. (http://codemaker.net)
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using System.Xml;

namespace EppLib.Entities
{
    public abstract class EppExtension 
    {
		protected abstract string Namespace { get; set; }

    	public abstract XmlNode ToXml(XmlDocument doc);

        protected XmlElement CreateElement(XmlDocument doc, string qualifiedName)
        {
			return doc.CreateElement(qualifiedName, Namespace);
        }

        private XmlElement CreateElement(XmlDocument doc, string qualifiedName, string namespaceURI)
        {
            return doc.CreateElement(qualifiedName, namespaceURI);
        }

        protected XmlElement AddXmlElement(XmlDocument doc, XmlElement containingElement, String tagName, String value)
        {
            var xml_element = CreateElement(doc, tagName);

            if (!string.IsNullOrEmpty(value))
            {
                xml_element.AppendChild(doc.CreateTextNode(value));
            }
            containingElement.AppendChild(xml_element);
            return xml_element;
        }

        protected XmlElement AddXmlElement(XmlDocument doc, XmlElement containingElement, String tagName, String value, string namespaceUri)
        {
            var xml_element = CreateElement(doc, tagName, namespaceUri);

            if (!string.IsNullOrEmpty(value))
            {
                xml_element.AppendChild(doc.CreateTextNode(value));
            }
            containingElement.AppendChild(xml_element);
            return xml_element;
        }
    }
}