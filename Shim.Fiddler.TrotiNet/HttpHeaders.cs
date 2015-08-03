using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fiddler;
using System.IO;

namespace TrotiNet
{
    public class HttpHeaders
    {
        public HttpHeaders(HTTPRequestHeaders hdrs, Session sess)
        {
            Headers = new Dictionary<string, string>();
            foreach (var hdr in hdrs) {
                Headers[hdr.Name.ToLowerInvariant()] = hdr.Value;
            }
            using (var buffer = new MemoryStream()) {
                sess.WriteRequestToStream(true, true, buffer);
                buffer.Flush();
                HeadersInOrder = Encoding.ASCII.GetString(buffer.ToArray());
            }

            CacheControl = hdrs["Cache-Control"];
            Connection = hdrs["Connection"]?.Split(';').Select(x => x.Trim()).ToArray();
            ContentEncoding = hdrs["Content-Encoding"];
            ContentLength = string.IsNullOrWhiteSpace(hdrs["Content-Length"]) ? (uint?)null : uint.Parse(hdrs["Content-Length"]);
            Expires = hdrs["Expires"];
            Host = hdrs["Host"];
            Pragma = hdrs["Pragma"];
            ProxyConnection = hdrs["Proxy-Connection"]?.Split(';').Select(x => x.Trim()).ToArray();
            Referer = hdrs["Referer"];
            TransferEncoding = hdrs["Transfer-Encoding"]?.Split(';').Select(x => x.Trim()).ToArray();
        }

        public HttpHeaders(HTTPResponseHeaders hdrs, Session sess)
        {
            Headers = new Dictionary<string, string>();
            foreach(var hdr in hdrs) {
                Headers[hdr.Name.ToLowerInvariant()] = hdr.Value;
            }
            using (var buffer = new MemoryStream()) {
                sess.WriteRequestToStream(true, true, buffer);
                buffer.Flush();
                HeadersInOrder = Encoding.ASCII.GetString(buffer.ToArray());
            }

            CacheControl = hdrs["Cache-Control"];
            Connection = hdrs["Connection"]?.Split(';').Select(x => x.Trim()).ToArray();
            ContentEncoding = hdrs["Content-Encoding"];
            ContentLength = string.IsNullOrWhiteSpace(hdrs["Content-Length"]) ? (uint?)null : uint.Parse(hdrs["Content-Length"]);
            Expires = hdrs["Expires"];
            Host = hdrs["Host"];
            Pragma = hdrs["Pragma"];
            ProxyConnection = hdrs["Proxy-Connection"]?.Split(';').Select(x => x.Trim()).ToArray();
            Referer = hdrs["Referer"];
            TransferEncoding = hdrs["Transfer-Encoding"]?.Split(';').Select(x => x.Trim()).ToArray();
        }
        
        public string CacheControl { get; set; }

        public string[] Connection { get; set; }

        public string ContentEncoding { get; set; }

        public uint? ContentLength { get; set; }

        public string Expires { get; set; }

        public Dictionary<string, string> Headers { get; protected set; }

        public string HeadersInOrder { get; protected set; }

        public string Host { get; set; }

        public string Pragma { get; set; }

        public string[] ProxyConnection { get; set; }

        public string Referer { get; set; }

        public string[] TransferEncoding { get; set; }
    }
}
