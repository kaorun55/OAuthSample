using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace OAuthSample
{
    class Program
    {
        // 公開しないようにする
        const string ConsumerKey = "OKWyYVPvnpcBfbdmrJaNWx";
        const string ConsumerSecret = "wObshonqbOqrnhIXGwflmlEVUFiiEJqScYLmKbPYTe";

        const string RequestServer = "https://cacoo.com/api";

        // アクセストークン
        const string AccessToken = "https://cacoo.com/oauth/access_token";
        const string Authorize = "https://cacoo.com/oauth/authorize";
        const string ReqestToken = "https://cacoo.com/oauth/request_token";

        const string Diagrams = "https://cacoo.com/api/v1/users/kaorun55.xml";

        static void Main(string[] args)
        {
            try
            {
                System.Net.ServicePointManager.Expect100Continue = false;
                OAuth.OAuthBase oauth = new OAuth.OAuthBase();

                string token = "";
                string tokenSecret = "";
                string PIN = "";

                token = "d14c062a4b597b03f2555f77f1d894c0";
                tokenSecret = "ddaa5235c48165a57941fec0a88f89c9";
                PIN = "8503307";

                Uri uri = new Uri(Diagrams);
                string normalizedUrl, normalizedRequestParameters;
                string signature = oauth.GenerateSignature(uri, ConsumerKey, ConsumerSecret, token, tokenSecret,
                    "POST", oauth.GenerateTimeStamp(), oauth.GenerateNonce(), OAuth.OAuthBase.SignatureTypes.HMACSHA1,
                    out normalizedUrl, out normalizedRequestParameters);

                string requeset = string.Format("{2}&oauth_signature={0}&oauth_verifier={1}", signature, PIN, normalizedRequestParameters);
                string requesetUrl = Diagrams + "?" + requeset;
                Process.Start(requesetUrl);
                Console.WriteLine(requeset);
                HttpWebRequest webreq = (System.Net.HttpWebRequest)WebRequest.Create(requesetUrl);
                webreq.Headers.Set("Authorization", "OAuth " + requeset);

                //oauth_token,oauth_token_secretの取得
                webreq.Method = "POST";
                HttpWebResponse webres = (System.Net.HttpWebResponse)webreq.GetResponse();

                string result;
                using (System.IO.Stream st = webres.GetResponseStream())
                using (System.IO.StreamReader sr = new System.IO.StreamReader(st, Encoding.GetEncoding(932)))
                {
                    result = sr.ReadToEnd();
                }

                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.Write("Press enter : ");
            Console.ReadLine();
        }
    }
}
