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
        const string Diagrams = "http://cacoo.com/api/v1/diagrams.xml";

        static void Main( string[] args )
        {
            try {
                System.Net.ServicePointManager.Expect100Continue = false;
                OAuth.OAuthBase oauth = new OAuth.OAuthBase();

                Uri uri = new Uri( Diagrams );
                string normalizedUrl, normalizedRequestParameters, s;

                string timestamp = oauth.GenerateTimeStamp();
                string nonce = oauth.GenerateNonce();

                //timestamp = "1281598828";
                //nonce = "4642986830566840008";

                string signature = oauth.GenerateSignature( uri, OAuth.APIKey.ConsumerKey, OAuth.APIKey.ConsumerSecret,
                    OAuth.APIKey.Token, OAuth.APIKey.TokenSecret,
                    "POST", timestamp, nonce, OAuth.OAuthBase.SignatureTypes.HMACSHA1,
                    "", out normalizedUrl, out normalizedRequestParameters, out s );

                HttpWebRequest webreq = (System.Net.HttpWebRequest)WebRequest.Create( Diagrams );
                webreq.Method = "POST";
                webreq.Headers.Add( "Authorization", "OAuth " + s );
                webreq.UserAgent = "Java/1.6.0_20";
                webreq.Accept = "text/html, image/gif, image/jpeg, *; q=.2, */*; q=.2";
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
