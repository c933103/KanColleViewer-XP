using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrotiNet
{
    public class HttpRequestLine
    {
        public HttpRequestLine(Fiddler.Session sess)
        {
            Method = sess.RequestMethod;
            ProtocolVersion = sess.RequestHeaders.HTTPVersion;
            URI = sess.fullUrl;
            RequestLine = string.Format("{0} {1} {2}", Method, URI, ProtocolVersion);
        }

        public string Method { get; set; }
        public string ProtocolVersion { get; set; }
        public string RequestLine { get; protected set; }
        public string URI { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
