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
using System.Collections.Generic;
using System.Xml;

namespace EppLib.Entities
{
    public class DomainRelease : DomainBase<DomainReleaseResponse>
    {
        private string _domainName;
        private string _registrarTag;

        public DomainRelease(string domainName, string registrarTag)
        {
        	_domainName = domainName;
        	_registrarTag = registrarTag;
        }

        protected override XmlElement BuildCommandElement(XmlDocument doc, XmlElement commandRootElement)
        {
            var domainRelease = BuildCommandElement(doc, "update", commandRootElement);
            
			var releaseNode = AddXmlElement(doc, domainRelease, "r:release", null, "http://www.nominet.org.uk/epp/xml/std-release-1.0");

            AddXmlElement(doc, releaseNode, "r:domainName", _domainName, "http://www.nominet.org.uk/epp/xml/std-release-1.0");
            AddXmlElement(doc, releaseNode, "r:registrarTag", _registrarTag, "http://www.nominet.org.uk/epp/xml/std-release-1.0");
            
            return domainRelease;
        }

		public override DomainReleaseResponse FromBytes(byte[] bytes)
        {
			return new DomainReleaseResponse(bytes);
        }

        private XmlElement BuildCommandElement(XmlDocument doc, string qualifiedName, XmlElement commandRootElement)
        {
            var elem = CreateElement(doc, qualifiedName);

            commandRootElement.AppendChild(elem);

            return elem;
        }
    }
}
