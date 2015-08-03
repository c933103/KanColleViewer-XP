using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiddler;

namespace TrotiNet
{
    public class HttpStatusLine
    {
        public readonly string StatusLine;

        public HttpStatusLine(Session sess)
        {
            StatusCode = sess.ResponseHeaders.HTTPResponseCode;
            ProtocolVersion = sess.ResponseHeaders.HTTPVersion;
            StatusLine = string.Format("{0} {1}", sess.ResponseHeaders.HTTPResponseStatus, sess.ResponseHeaders.HTTPVersion);
        }

        public string ProtocolVersion { get; protected set; }

        public int StatusCode { get; protected set; }
        
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
