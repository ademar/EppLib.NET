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
    public abstract class DomainBase<T> : EppCommand<T> where T : EppResponse
    {
        protected DomainBase()
            : base("domain", "urn:ietf:params:xml:ns:domain-1.0")
        {
        }

        protected XmlNode CreateNameServerElement(XmlDocument doc, IEnumerable<string> nameServers)
        {
            var nameServerElement = doc.CreateElement("domain:ns", namespaceUri);

            foreach (var serverName in nameServers)
            {
                AddXmlElement(doc, nameServerElement, "domain:hostObj", serverName,namespaceUri);
            }

            return nameServerElement;
        }
    }
}
