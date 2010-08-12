using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;
using OAuth;
using System.IO;

namespace TwitterOAuthGetAccessToken
{
    class Program
    {
        static void Main( string[] args )
        {
            try {
                getAccessToken( OAuth.APIKey.ConsumerKey, OAuth.APIKey.ConsumerSecret );
            }
            catch ( Exception ex ) {
                Console.WriteLine( ex.Message );
            }
        }

        private static void getAccessToken( string consumer_key, string consumer_secret )
        {
            System.Net.ServicePointManager.Expect100Continue = false;
            OAuthBase oAuth = new OAuthBase();
            string nonce = oAuth.GenerateNonce();

            System.Uri uri = new Uri( OAuth.APIKey.ReqestToken );
            string timestamp = oAuth.GenerateTimeStamp();


            //OAuthBace.csを用いてsignature生成
            string normalizedUrl, normalizedRequestParameters, s;
            string signature = oAuth.GenerateSignature( uri, consumer_key, consumer_secret, "", "", "GET", timestamp, nonce,
                OAuthBase.SignatureTypes.HMACSHA1, "", out normalizedUrl, out normalizedRequestParameters, out s );


            //oauth_token,oauth_token_secret取得
            HttpWebRequest webreq = (System.Net.HttpWebRequest)WebRequest.Create( OAuth.APIKey.ReqestToken + string.Format( "?{0}&oauth_signature={1}", normalizedRequestParameters, signature ) );
            webreq.Method = "GET";
            HttpWebResponse webres = (HttpWebResponse)webreq.GetResponse();

            string result = "";
            using ( System.IO.Stream st = webres.GetResponseStream() )
            using ( System.IO.StreamReader sr = new System.IO.StreamReader( st, Encoding.GetEncoding( 932 ) ) ) {
                result = sr.ReadToEnd();
            }

            Console.WriteLine( result );

            //正規表現でoauth_token,oauth_token_secret取得
            Match match = Regex.Match( result, @"oauth_token=(.*?)&oauth_token_secret=(.*?)&oauth_callback.*" );
            string token = match.Groups[1].Value;
            string tokenSecret = match.Groups[2].Value;


            //ブラウザからPIN確認
            string AuthorizeURL = OAuth.APIKey.Authorize + "?" + result;
            System.Diagnostics.Process.Start( AuthorizeURL );
            Console.Write( "PIN:" );
            string PIN = Console.ReadLine();

            //oauth_token,oauth_token_secretを用いて再びsignature生成
            uri = new Uri( OAuth.APIKey.AccessToken );
            signature = oAuth.GenerateSignature( uri, consumer_key, consumer_secret, token, tokenSecret, "POST",
                    oAuth.GenerateTimeStamp(), oAuth.GenerateNonce(), OAuthBase.SignatureTypes.HMACSHA1, PIN,
                    out normalizedUrl, out normalizedRequestParameters, out s );

            string request = OAuth.APIKey.AccessToken + string.Format( "?{3}&oauth_signature={0}", signature, result, PIN, normalizedRequestParameters );
            Console.WriteLine( normalizedRequestParameters );
            Console.WriteLine( signature );
            webreq = (System.Net.HttpWebRequest)WebRequest.Create( request );


            //oauth_token,oauth_token_secretの取得
            webreq.Method = "POST";
            webres = (System.Net.HttpWebResponse)webreq.GetResponse();

            using ( System.IO.Stream st = webres.GetResponseStream() )
            using ( System.IO.StreamReader sr = new System.IO.StreamReader( st, Encoding.GetEncoding( 932 ) ) ) {
                result = sr.ReadToEnd();
            }

            Console.WriteLine( result );

            //正規表現でoauth_token,oauth_token_secret取得
            match = Regex.Match( result, @"oauth_token=(.*)&oauth_token_secret=(.*)" );
            token = match.Groups[1].Value;
            tokenSecret = match.Groups[2].Value;

            Console.WriteLine( "public const string Token = \"" + token + "\";" );
            Console.WriteLine( "public const string TokenSecret = \"" + tokenSecret + "\";" );

            //デスクトップ\oauth_token.txtに保存
            File.WriteAllText( Environment.GetFolderPath( Environment.SpecialFolder.Desktop ) + @"\cacoo_oauth_token.txt", result );
        }
    }
}
