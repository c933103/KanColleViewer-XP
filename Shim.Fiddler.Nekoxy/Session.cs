using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fiddler;

namespace Nekoxy
{
    public class Session
    {
        public Session(Fiddler.Session sess)
        {
            Request = new HttpRequest(sess);
            Response = new HttpResponse(sess);
        }

        public HttpRequest Request { get; internal set; }
        
        public HttpResponse Response { get; internal set; }
    }
}
