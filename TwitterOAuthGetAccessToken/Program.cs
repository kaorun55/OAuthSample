using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace TwitterOAuthGetAccessToken
{
    class Program
    {
        static void Main( string[] args )
        {
            try {
                System.Net.ServicePointManager.Expect100Continue = false;
                OAuth.OAuthBase oauth = new OAuth.OAuthBase();
                string nonce = oauth.GenerateNonce();

                System.Uri uri = new Uri( OAuth.APIKey.ReqestToken );
                string timestamp = oauth.GenerateTimeStamp();

                string normalizedUrl, normalizedRequestParameters, authorationRequestParameters;
                string signature = oauth.GenerateSignature( uri, OAuth.APIKey.ConsumerKey, OAuth.APIKey.ConsumerSecret, "", "", "GET", timestamp,
                            nonce, OAuth.OAuthBase.SignatureTypes.HMACSHA1, out normalizedUrl, out normalizedRequestParameters, out authorationRequestParameters );

                //oauth_token,oauth_token_secret取得
                string request = OAuth.APIKey.ReqestToken + string.Format( "?{0}&oauth_signature={1}", normalizedRequestParameters, signature );
                HttpWebRequest webreq = (System.Net.HttpWebRequest)WebRequest.Create( request );

                webreq.Method = "GET";
                HttpWebResponse webres = (HttpWebResponse)webreq.GetResponse();

                string result;
                using ( System.IO.Stream st = webres.GetResponseStream() )
                using ( System.IO.StreamReader sr = new System.IO.StreamReader( st, Encoding.GetEncoding( 932 ) ) ) {
                    result = sr.ReadToEnd();
                }

                Match match = Regex.Match( result, @"oauth_token=(.*?)&oauth_token_secret=(.*?)&oauth_callback.*" );
                string token = match.Groups[1].Value;
                string tokenSecret = match.Groups[2].Value;


                //ブラウザからPIN確認
                string AuthorizeURL = OAuth.APIKey.Authorize + "?oauth_token=" + token;
                Process.Start( AuthorizeURL );
                Console.Write( "PIN:" );
                string PIN = Console.ReadLine();

                Console.WriteLine( "public const string token = \"" + token + "\";" );
                Console.WriteLine( "public const string tokenSecret = \"" + tokenSecret + "\";" );
                Console.WriteLine( "public const string PIN = \"" + PIN + "\";" );

                Console.Write( "Press enter : " );
                Console.ReadLine();
            }
            catch ( Exception ex ) {
                Console.WriteLine( ex.Message );
            }
        }
    }
}
