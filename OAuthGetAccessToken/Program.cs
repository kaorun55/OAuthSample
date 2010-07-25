using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Diagnostics;

namespace OAuthGetAccessToken
{
    class Program
    {
        // 公開しないようにする
        const string ConsumerKey = "";
        const string ConsumerSecret = "";


        // アクセストークン
        const string AccessToken = "https://cacoo.com/oauth/access_token";
        const string Authorize = "https://cacoo.com/oauth/authorize";
        const string ReqestToken = "https://cacoo.com/oauth/request_token";

        const string Diagrams = "https://cacoo.com/api/v1/diagrams.json";

        static void Main(string[] args)
        {
            try
            {
                System.Net.ServicePointManager.Expect100Continue = false;
                OAuth.OAuthBase oauth = new OAuth.OAuthBase();
                string nonce = oauth.GenerateNonce();

                System.Uri uri = new Uri(ReqestToken);
                string timestamp = oauth.GenerateTimeStamp();

                string normalizedUrl, normalizedRequestParameters;
                string signature = oauth.GenerateSignature(uri, ConsumerKey, ConsumerSecret, "", "", "GET", timestamp,
                            nonce, OAuth.OAuthBase.SignatureTypes.HMACSHA1, out normalizedUrl, out normalizedRequestParameters);

                //oauth_token,oauth_token_secret取得
                string request = ReqestToken + string.Format("?{0}&oauth_signature={1}", normalizedRequestParameters, signature);
                HttpWebRequest webreq = (System.Net.HttpWebRequest)WebRequest.Create(request);

                webreq.Method = "GET";
                HttpWebResponse webres = (HttpWebResponse)webreq.GetResponse();

                string result;
                using (System.IO.Stream st = webres.GetResponseStream())
                using (System.IO.StreamReader sr = new System.IO.StreamReader(st, Encoding.GetEncoding(932)))
                {
                    result = sr.ReadToEnd();
                }

                Match match = Regex.Match(result, @"oauth_token=(.*?)&oauth_token_secret=(.*?)&oauth_callback.*");
                string token = match.Groups[1].Value;
                string tokenSecret = match.Groups[2].Value;


                //ブラウザからPIN確認
                string AuthorizeURL = Authorize + "?oauth_token=" + token;
                Process.Start(AuthorizeURL);
                Console.Write("PIN:");
                string PIN = Console.ReadLine();

                Console.WriteLine("token = \"" + token + "\";");
                Console.WriteLine("tokenSecret = \"" + tokenSecret + "\";");
                Console.WriteLine("PIN = \"" + PIN + "\";");

                Console.Write("Press enter : ");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
