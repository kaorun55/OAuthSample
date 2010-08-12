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
        /// リクエストトークンを取得する
        /// </summary>
        /// <param name="consumer"></param>
        /// <returns></returns>
        public string RetrieveRequestToken( ref OAuthConsumer consumer )
        {
            //OAuthBace.csを用いてsignature生成
            string signature = oauth.GenerateSignature( new Uri( ReqestToken ), consumer, GET, "" );

            //oauth_token,oauth_token_secret取得
            string request = string.Format( "?{0}&oauth_signature={1}", oauth.NormalizedRequestParameters, signature );
            string requestURL = ReqestToken + request;
            HttpWebRequest webreq = (System.Net.HttpWebRequest)WebRequest.Create( requestURL );
            webreq.Method = GET;

            HttpWebResponse webres = (HttpWebResponse)webreq.GetResponse();
            string result = "";
            using ( System.IO.Stream st = webres.GetResponseStream() )
            using ( System.IO.StreamReader sr = new System.IO.StreamReader( st, Encoding.UTF8 ) ) {
                result = sr.ReadToEnd();
            }

            //正規表現でoauth_token,oauth_token_secret取得
            Match match = Regex.Match( result, @"oauth_token=(.*?)&oauth_token_secret=(.*?)&oauth_callback.*" );
            consumer.SetTokenWithSecret( match.Groups[1].Value, match.Groups[2].Value );

            // 認証用URLを返す
            return Authorize + "?" + result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consumer"></param>
        /// <param name="pin"></param>
        public void RetrieveAccessToken( ref OAuthConsumer consumer, string pin )
        {
            //oauth_token,oauth_token_secretを用いて再びsignature生成
            Uri uri = new Uri( AccessToken );
            string signature = oauth.GenerateSignature( uri, consumer, POST, pin );

            string request = AccessToken + string.Format( "?{1}&oauth_signature={0}", signature, oauth.NormalizedRequestParameters );
            HttpWebRequest webreq = (System.Net.HttpWebRequest)WebRequest.Create( request );
            webreq.Method = POST;

            HttpWebResponse webres = (System.Net.HttpWebResponse)webreq.GetResponse();
            string result = "";
            using ( System.IO.Stream st = webres.GetResponseStream() )
            using ( System.IO.StreamReader sr = new System.IO.StreamReader( st, Encoding.UTF8 ) ) {
                result = sr.ReadToEnd();
            }

            Console.WriteLine( result );

            //正規表現でoauth_token,oauth_token_secret取得
            Match match = Regex.Match( result, @"oauth_token=(.*)&oauth_token_secret=(.*)" );
            consumer.SetTokenWithSecret( match.Groups[1].Value, match.Groups[2].Value );
        }
    }
}
