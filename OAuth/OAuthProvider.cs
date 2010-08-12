using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;

namespace OAuth
{
    public class OAuthProvider
    {
        private OAuthBase oauth = new OAuthBase();

        private const string GET = "GET";
        private const string POST = "POST";

        private const string RequestFormatAuth = "?{0}&oauth_signature={1}";
        private const string RequestFormatRequest = "";

        /// <summary>
        /// 
        /// </summary>
        public string AccessToken
        {
            private set;
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Authorize
        {
            private set;
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ReqestToken
        {
            private set;
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AccessToken"></param>
        /// <param name="Authorize"></param>
        /// <param name="ReqestToken"></param>
        public OAuthProvider( string AccessToken, string Authorize, string ReqestToken )
        {
            this.AccessToken = AccessToken;
            this.Authorize = Authorize;
            this.ReqestToken = ReqestToken;
        }

        /// <summary>
        /// 
        /// </summary>
        public OAuthProvider() : this( "", "", "" )
        {
        }

        /// <summary>
        /// リクエストトークンを取得する
        /// </summary>
        /// <param name="consumer"></param>
        /// <returns></returns>
        public string RetrieveRequestToken( ref OAuthConsumer consumer )
        {
            // リクエストの作成とレスポンスの取得
            HttpWebRequest webreq = CreateRequest( ReqestToken, consumer, GET, "", RequestFormatAuth );
            string result = GetResponse( webreq );

            // oauth_tokenとoauth_token_secretを取得
            Match match = Regex.Match( result, @"oauth_token=(.*?)&oauth_token_secret=(.*?)&oauth_callback.*" );
            consumer.SetTokenWithSecret( match.Groups[1].Value, match.Groups[2].Value );

            // 認証用URLを返す
            return Authorize + "?" + result;
        }

        /// <summary>
        /// アクセストークンの取得
        /// </summary>
        /// <param name="consumer"></param>
        /// <param name="pin"></param>
        public void RetrieveAccessToken( ref OAuthConsumer consumer, string pin )
        {
            // リクエストの作成とレスポンスの取得
            HttpWebRequest webreq = CreateRequest( AccessToken, consumer, POST, pin, RequestFormatAuth );
            string result = GetResponse( webreq );

            // oauth_tokenとoauth_token_secretを取得
            Match match = Regex.Match( result, @"oauth_token=(.*)&oauth_token_secret=(.*)" );
            consumer.SetTokenWithSecret( match.Groups[1].Value, match.Groups[2].Value );
        }

        /// <summary>
        /// リクエストを取得
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="consumer"></param>
        /// <returns></returns>
        public string RetrieveRequest( string uri, OAuthConsumer consumer )
        {
            HttpWebRequest webreq = CreateRequest( uri, consumer, POST, "", RequestFormatRequest );
            webreq.Headers.Add( "Authorization", "OAuth " + oauth.AuthorizationRequestParameters );

            return GetResponse( webreq );
        }

        /// <summary>
        /// リクエストの作成
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="consumer"></param>
        /// <param name="method"></param>
        /// <param name="pin"></param>
        /// <returns></returns>
        private HttpWebRequest CreateRequest( string uri, OAuthConsumer consumer, string method, string pin, string format )
        {
            string signature = oauth.GenerateSignature( new Uri( uri ), consumer, method, pin );

            string request = string.Format( format, oauth.NormalizedRequestParameters, signature );
            string requestURL = uri + request;
            HttpWebRequest webreq = (System.Net.HttpWebRequest)WebRequest.Create( requestURL );
            webreq.Method = method;

            return webreq;
        }

        /// <summary>
        /// レスポンスの取得
        /// </summary>
        /// <param name="webreq"></param>
        /// <returns></returns>
        private static string GetResponse( HttpWebRequest webreq )
        {
            HttpWebResponse webres = (HttpWebResponse)webreq.GetResponse();
            using ( System.IO.Stream st = webres.GetResponseStream() )
            using ( System.IO.StreamReader sr = new System.IO.StreamReader( st, Encoding.UTF8 ) ) {
                return sr.ReadToEnd();
            }
        }
    }
}
