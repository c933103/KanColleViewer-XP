using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiddler;
using TrotiNet;

namespace Nekoxy
{
    public class HttpResponse
    {
        /// <summary>
        /// HTTPステータス、ヘッダ、ボディを元に初期化。
        /// </summary>
        /// <param name="statusLine">HTTPステータスライン。</param>
        /// <param name="headers">HTTPレスポンスヘッダ。</param>
        /// <param name="body">HTTPレスポンスボディ。</param>
        public HttpResponse(HttpStatusLine statusLine, HttpHeaders headers, byte[] body)
        {
            this.StatusLine = statusLine;
            this.Headers = headers;
            this.Body = body;
        }

        public HttpResponse(Fiddler.Session sess)
        {
            sess.utilDecodeResponse();

            this.StatusLine = new HttpStatusLine(sess);
            this.Body = (byte[])sess.responseBodyBytes.Clone();
            this.Headers = new HttpHeaders(sess.ResponseHeaders, sess);
        }

        /// <summary>
        /// HTTPステータスライン。
        /// </summary>
        public HttpStatusLine StatusLine { get; }

        /// <summary>
        /// HTTPヘッダヘッダ。
        /// </summary>
        public HttpHeaders Headers { get; }

        /// <summary>
        /// HTTPレスポンスボディ。
        /// </summary>
        public byte[] Body { get; }

        /// <summary>
        /// content-type ヘッダ。
        /// </summary>
        public string ContentType
            => this.Headers.Headers.ContainsKey("content-type")
            ? this.Headers.Headers["content-type"]
            : string.Empty;

        /// <summary>
        /// content-type ヘッダから MIME Type のみ取得。
        /// </summary>
        public string MimeType => this.ContentType.GetMimeType();

        /// <summary>
        /// レスポンスの文字エンコーディング。
        /// content-typeヘッダに指定されたcharsetを元に生成される。
        /// 指定がない場合はUS-ASCII。
        /// </summary>
        public Encoding Charset => this.Headers.GetEncoding();

        /// <summary>
        /// HTTPレスポンスボディを文字列で取得する。
        /// </summary>
        public string BodyAsString => this.Charset.GetString(this.Body);
    }
}
