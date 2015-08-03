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
        public HttpHeaders(HTTPRequestHeaders hdr, Session sess)
        {
            Headers = hdr.ToDictionary(x => x.Name, x => x.Value);
            using (var buffer = new MemoryStream()) {
                sess.WriteRequestToStream(true, true, buffer);
                buffer.Flush();
                HeadersInOrder = Encoding.ASCII.GetString(buffer.ToArray());
            }

            CacheControl = hdr["Cache-Control"];
            Connection = hdr["Connection"]?.Split(';').Select(x => x.Trim()).ToArray();
            ContentEncoding = hdr["Content-Encoding"];
            ContentLength = hdr["Content-Length"] == null ? (uint?)null : uint.Parse(hdr["Content-Length"]);
            Expires = hdr["Expires"];
            Host = hdr["Host"];
            Pragma = hdr["Pragma"];
            ProxyConnection = hdr["Proxy-Connection"]?.Split(';').Select(x => x.Trim()).ToArray();
            Referer = hdr["Referer"];
            TransferEncoding = hdr["Transfer-Encoding"]?.Split(';').Select(x => x.Trim()).ToArray();
        }

        public HttpHeaders(HTTPResponseHeaders hdr, Session sess)
        {
            Headers = hdr.ToDictionary(x => x.Name, x => x.Value);
            using (var buffer = new MemoryStream()) {
                sess.WriteRequestToStream(true, true, buffer);
                buffer.Flush();
                HeadersInOrder = Encoding.ASCII.GetString(buffer.ToArray());
            }

            CacheControl = hdr["Cache-Control"];
            Connection = hdr["Connection"]?.Split(';').Select(x => x.Trim()).ToArray();
            ContentEncoding = hdr["Content-Encoding"];
            ContentLength = hdr["Content-Length"] == null ? (uint?)null : uint.Parse(hdr["Content-Length"]);
            Expires = hdr["Expires"];
            Host = hdr["Host"];
            Pragma = hdr["Pragma"];
            ProxyConnection = hdr["Proxy-Connection"]?.Split(';').Select(x => x.Trim()).ToArray();
            Referer = hdr["Referer"];
            TransferEncoding = hdr["Transfer-Encoding"]?.Split(';').Select(x => x.Trim()).ToArray();
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
