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
using System.Collections.Generic;
using System.Xml;
using EppLib.Entities;

namespace EppLib.Extensions.Cira
{
    public class CiraDomainTransfer : DomainTransfer
    {
        public CiraDomainTransfer(string mName) : base(mName)
        {
        }

        public CiraDomainTransfer(string mName, string registrantContactId, string adminContactId, string techContactId) : base(mName, registrantContactId, adminContactId, techContactId)
        {
        }

        public CiraDomainTransfer(string mName, string registrantContactId, string adminContactId, IList<string> techContactIds) : base(mName, registrantContactId, adminContactId, techContactIds)
        {
        }

        public override XmlDocument ToXml()
        {
            var ciraExtension = new CiraTransferExtension
                                    {
                                        RegistrantContactId = registrantContactId,
                                        AdminContactId = adminContactId,
                                        TechContactIds = techContactIds
                                    };

            Extensions.Clear();
            Extensions.Add(ciraExtension);

            return base.ToXml();
        }

    }
}
