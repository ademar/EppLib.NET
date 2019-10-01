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
using System.Diagnostics;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace EppLib
{
    public class WebConnection : ITransport
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _schema;
        private HttpClient _client;
        private readonly int _readTimeout;

        /// <summary>
        /// Stores the response of the latest write ready to be fetched by the read command
        /// </summary>
        private string _resultBuffer;

        private readonly TraceSource _traceSource = new TraceSource("EppLib");

        public WebConnection(string host, int port, string schema = "https", int readTimeout = Timeout.Infinite)
        {
            _host = host;
            _port = port;
            _readTimeout = readTimeout;
            _schema = schema;

            _host = _host.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase)
                ? _host.Replace("https://", string.Empty)
                : _host;

            _host = _host.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase)
                ? _host.Replace("http://", string.Empty)
                : _host;

            _traceSource.TraceInformation($"Set connection to: {_schema}://{_host}:{_port}");
        }

        public void Connect(SslProtocols sslProtocols = SslProtocols.Tls)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri($"{_schema}://{_host}:{_port}"),
                Timeout = TimeSpan.FromMilliseconds(_readTimeout)
            };
        }

        public void Disconnect()
        {
        }

        public void Dispose()
        {
            _client?.Dispose();
        }


        /// <summary>
        /// Writes a command to the EPP server and stores the result in an internal buffer.
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        public void Write(XmlDocument xmlDocument)
        {
            _traceSource.TraceData(TraceEventType.Information, 0, xmlDocument.OuterXml);
            var post = _client.PostAsync("", new StringContent(xmlDocument.OuterXml))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            if (!post.IsSuccessStatusCode)
            {
                throw new Exception("HTTP status = " + post.StatusCode);
            }

            _resultBuffer = post.Content.ReadAsStringAsync()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }


        /// <summary>
        /// Reads the internal buffer from the latest response.
        /// </summary>
        /// <returns></returns>
        public byte[] Read()
        {
            _traceSource.TraceData(TraceEventType.Information, 0, _resultBuffer);
            var result = GetBytes(_resultBuffer);
            _resultBuffer = null;
            return result;
        }


        private static byte[] GetBytes(string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }
    }
}
