using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;

namespace OAuthSample
{
    class Program
    {
        // アクセストークン
        const string AccessToken = "https://cacoo.com/oauth/access_token";
        const string Authorize = "https://cacoo.com/oauth/authorize";
        const string ReqestToken = "https://cacoo.com/oauth/request_token";

        const string Diagrams = "http://cacoo.com/api/v1/users/kaorun55.xml";

        static void Main( string[] args )
        {
            try {
                System.Net.ServicePointManager.Expect100Continue = false;
                OAuth.OAuthBase oauth = new OAuth.OAuthBase();

                Uri uri = new Uri( Diagrams );
                string normalizedUrl, normalizedRequestParameters, authorationRequestParameters;
                string signature = oauth.GenerateSignature( uri, OAuth.APIKey.ConsumerKey, OAuth.APIKey.ConsumerSecret,
                    OAuth.APIKey.Token, OAuth.APIKey.TokenSecret,
                    "POST", oauth.GenerateTimeStamp(), oauth.GenerateNonce(), OAuth.OAuthBase.SignatureTypes.HMACSHA1,
                    "", out normalizedUrl, out normalizedRequestParameters );

                Console.WriteLine( normalizedUrl );
                Console.WriteLine( normalizedRequestParameters );

                //string requeset = string.Format( "{2}&oauth_signature={0}&oauth_verifier={1}", signature, OAuth.APIKey.PIN, normalizedRequestParameters );
                string requeset = string.Format( "{2}", signature, OAuth.APIKey.PIN, normalizedRequestParameters );
                string requesetUrl = Diagrams + "?" + requeset;
                HttpWebRequest webreq = (System.Net.HttpWebRequest)WebRequest.Create( Diagrams );
                webreq.Method = "POST";
                webreq.ContentType = "application/x-www-form-urlencoded";

//                byte[] byteArray = Encoding.UTF8.GetBytes( requesetUrl );
//                Stream dataStream = webreq.GetRequestStream();
//                dataStream.Write( byteArray, 0, byteArray.Length );
//                dataStream.Close();
    
                HttpWebResponse webres = (System.Net.HttpWebResponse)webreq.GetResponse();

                string result;
                using ( System.IO.Stream st = webres.GetResponseStream() )
                using ( System.IO.StreamReader sr = new System.IO.StreamReader( st, Encoding.GetEncoding( 932 ) ) ) {
                    result = sr.ReadToEnd();
                }

                Console.WriteLine( result );
            }
            catch ( Exception ex ) {
                Console.WriteLine( ex.Message );
            }

            Console.Write( "Press enter : " );
            Console.ReadLine();
        }
    }
}
