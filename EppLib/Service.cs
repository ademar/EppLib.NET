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

using System.Security.Authentication;
using System.Xml;
using EppLib.Entities;

namespace EppLib
{
	/// <summary>
	/// Encapsulates the EPP protocol
	/// </summary>
	public class Service
	{
		private readonly ITransport transport;

		public Service(ITransport transport)
		{
			this.transport = transport;
		}


		/// <summary>
		/// Connects to the registry end point
		/// </summary>
		public void Connect(SslProtocols sslProtocols = SslProtocols.Tls)
		{
			transport.Connect(sslProtocols);
			transport.Read();

		}

		/// <summary>
		/// Executes an EPP command
		/// </summary>
		/// <param name="command">The EPP command</param>
		/// <returns></returns>
		public T Execute<T>(EppBase<T> command) where T : EppResponse
		{
			byte[] bytes = SendAndReceive(command.ToXml());

			return command.FromBytes(bytes);
		}

		internal byte[] SendAndReceive(XmlDocument xmlDocument)
		{
			transport.Write(xmlDocument);

			return transport.Read();
		}

		/// <summary>
		/// Disconects from the registry end point
		/// </summary>
		public void Disconnect()
		{
			transport.Disconnect();
		}
	}
}
