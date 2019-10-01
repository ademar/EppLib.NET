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
using System.Net.Sockets;

using System.Text;
using System.Threading;
using System.Xml;
/**/
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;


/*
using OpenSSL;
using OpenSSL.Core;
using OpenSSL.X509;*/


namespace EppLib
{
    /// <summary>
    /// Encapsulates the TCP transport
    /// </summary>
    public class TcpTransport : ITransport
    {
        private SslStream stream;

        private readonly string EPP_REGISTRY_COM;
        private readonly int PORT;

        private readonly int READ_TIMEOUT;
        private readonly int WRITE_TIMEOUT;

        private readonly bool loggingEnabled;
        private readonly X509Certificate clientCertificate;

        public TcpTransport(string host, int port, X509Certificate clientCertificate, bool loggingEnabled = false, int readTimeout = Timeout.Infinite, int writeTimeout = Timeout.Infinite)
        {
            EPP_REGISTRY_COM = host;
            PORT = port;
            READ_TIMEOUT = readTimeout;
            WRITE_TIMEOUT = writeTimeout;
            this.loggingEnabled = loggingEnabled;
            this.clientCertificate = clientCertificate;
        }

        /// <summary>
        /// Connect to the registry end point
        /// </summary>
        public void Connect(SslProtocols sslProtocols)
        {
            var client = new TcpClient(EPP_REGISTRY_COM, PORT);

            stream = new SslStream(client.GetStream(), false, ValidateServerCertificate)
                     {
                         ReadTimeout = READ_TIMEOUT,
                         WriteTimeout = WRITE_TIMEOUT
                     };

            if (clientCertificate != null)
            {
                var clientCertificates = new X509CertificateCollection {clientCertificate};

                stream.AuthenticateAsClient(EPP_REGISTRY_COM, clientCertificates, sslProtocols, false);
            }
            else
            {
                stream.AuthenticateAsClient(EPP_REGISTRY_COM);
            }

        }

        
        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        /// <summary>
        /// Disconnect from the registry end point
        /// </summary>
        public void Disconnect()
        {
            stream.Close();
        }

        /// <summary>
        /// Read the command response
        /// </summary>
        /// <returns></returns>
        public byte[] Read()
        {
            var lenghtBytes = new byte[4];
            int read = 0;

            while (read < 4)
            {
                read = read + stream.Read(lenghtBytes, read, 4 - read);
            }

            Array.Reverse(lenghtBytes);

            var length = BitConverter.ToInt32(lenghtBytes, 0) - 4;

            if (loggingEnabled)
            {
                Debug.Log("Reading " + length + " bytes.");
            }

            var bytes = new byte[length];

            var returned = 0;

            while (returned != length)
            {
                returned += stream.Read(bytes, returned, length - returned);
            }

            if (loggingEnabled)
            {
                Debug.Log("****************** Received ******************");
                Debug.Log(bytes);
            }

            return bytes;
        }

        /// <summary>
        /// Writes an XmlDocument to the transport stream
        /// </summary>
        /// <param name="s"></param>
        public void Write(XmlDocument s)
        {
            var bytes = GetBytes(s);

            var lenght = bytes.Length + 4;

            var lenghtBytes = BitConverter.GetBytes(lenght);
            Array.Reverse(lenghtBytes);

            stream.Write(lenghtBytes, 0, 4);

            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();

            if (loggingEnabled)
            {
                Debug.Log("****************** Sending ******************");
                Debug.Log(bytes);
            }

        }

        private static byte[] GetBytes(XmlDocument s)
        {
            return Encoding.UTF8.GetBytes(s.OuterXml);
        }

        public void Dispose()
        {
            if (stream!=null)
            {
                stream.Dispose();
            }
        }
    }
}
