﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrotiNet;

namespace Nekoxy
{
    public class HttpRequest
    {
        public HttpRequest(Fiddler.Session sess)
        {
            RequestLine = new HttpRequestLine(sess);
            Body = (byte[])sess.requestBodyBytes.Clone();
            Headers = new HttpHeaders(sess.RequestHeaders, sess);
        }

        /// <summary>
        /// HTTPリクエストライン。
        /// </summary>
        public HttpRequestLine RequestLine { get; }

        /// <summary>
        /// HTTPヘッダ。
        /// </summary>
        public HttpHeaders Headers { get; }

        /// <summary>
        /// HTTPリクエストボディ。
        /// Transfer-Encoding: chunked なHTTPリクエストの RequestBody の読み取りは未対応。
        /// </summary>
        public byte[] Body { get; }

        /// <summary>
        /// パスとクエリ。
        /// </summary>
        public string PathAndQuery
            => this.RequestLine.URI.StartsWith("/")
            ? this.RequestLine.URI
            : new Uri(this.RequestLine.URI).PathAndQuery;

        /// <summary>
        /// リクエストの文字エンコーディング。
        /// content-typeヘッダに指定されたcharsetを元に生成される。
        /// 指定がない場合はUS-ASCII。
        /// </summary>
        public Encoding Charset => this.Headers.GetEncoding();

        /// <summary>
        /// HTTPリクエストボディを文字列で取得する。
        /// Transfer-Encoding: chunked なHTTPリクエストの RequestBody の読み取りは未対応。
        /// </summary>
        public string BodyAsString => this.Body != null ? this.Charset.GetString(this.Body) : null;
    }
}
