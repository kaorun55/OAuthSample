﻿using System;
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
            //try {
            //    System.Net.ServicePointManager.Expect100Continue = false;
            //    OAuth.OAuthBase oauth = new OAuth.OAuthBase();
            //    string nonce = oauth.GenerateNonce();

            //    System.Uri uri = new Uri( OAuth.APIKey.ReqestToken );
            //    string timestamp = oauth.GenerateTimeStamp();

            //    string normalizedUrl, normalizedRequestParameters, authorationRequestParameters;
            //    string signature = oauth.GenerateSignature( uri, OAuth.APIKey.ConsumerKey, OAuth.APIKey.ConsumerSecret, "", "", "GET", timestamp,
            //                nonce, OAuth.OAuthBase.SignatureTypes.HMACSHA1, out normalizedUrl, out normalizedRequestParameters, out authorationRequestParameters );

            //    string request = OAuth.APIKey.ReqestToken + string.Format( "?{0}&oauth_signature={1}", normalizedRequestParameters, signature );
            //    HttpWebRequest webreq = (System.Net.HttpWebRequest)WebRequest.Create( request );
            //    webreq.Method = "GET";

            //    //oauth_token,oauth_token_secret取得
            //    HttpWebResponse webres = (HttpWebResponse)webreq.GetResponse();

            //    string result;
            //    using ( System.IO.Stream st = webres.GetResponseStream() )
            //    using ( System.IO.StreamReader sr = new System.IO.StreamReader( st, Encoding.GetEncoding( 932 ) ) ) {
            //        result = sr.ReadToEnd();
            //    }

            //    Match match = Regex.Match( result, @"oauth_token=(.*?)&oauth_token_secret=(.*?)&oauth_callback.*" );
            //    string token = match.Groups[1].Value;
            //    string tokenSecret = match.Groups[2].Value;


            //    //ブラウザからPIN確認
            //    //string AuthorizeURL = OAuth.APIKey.Authorize + "?oauth_token=" + token;
            //    string AuthorizeURL = OAuth.APIKey.Authorize + "?" + result;
            //    Process.Start( AuthorizeURL );
            //    Console.Write( "PIN:" );
            //    string PIN = Console.ReadLine();

            //    //oauth_token,oauth_token_secretを用いて再びsignature生成
            //    signature = oauth.GenerateSignature( uri, OAuth.APIKey.ConsumerKey, OAuth.APIKey.ConsumerSecret, OAuth.APIKey.token,
            //                                        OAuth.APIKey.tokenSecret, "POST",
            //                                        oauth.GenerateTimeStamp(), oauth.GenerateNonce(), OAuth.OAuthBase.SignatureTypes.HMACSHA1,
            //                                        out normalizedUrl, out normalizedRequestParameters, out authorationRequestParameters );

            //    request = string.Format( "http://twitter.com/oauth/access_token?{3}&oauth_signature={0}&oauth_verifier={2}", signature, result, PIN, normalizedRequestParameters );
            //    Console.WriteLine( request );
            //    webreq = (System.Net.HttpWebRequest)WebRequest.Create( request );
            //    webreq.Method = "POST";

            //    webres = (System.Net.HttpWebResponse)webreq.GetResponse();

            //    using ( System.IO.Stream st = webres.GetResponseStream() )
            //    using ( System.IO.StreamReader sr = new System.IO.StreamReader( st, Encoding.GetEncoding( 932 ) ) ) {
            //        result = sr.ReadToEnd();
            //    }

            //    Console.WriteLine( result );

            //    Console.WriteLine( "public const string token = \"" + token + "\";" );
            //    Console.WriteLine( "public const string tokenSecret = \"" + tokenSecret + "\";" );
            //    Console.WriteLine( "public const string PIN = \"" + PIN + "\";" );

            //    Console.Write( "Press enter : " );
            //    Console.ReadLine();
            //}
            //catch ( Exception ex ) {
            //    Console.WriteLine( ex.Message );
            //}

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
            string signature = oAuth.GenerateSignature( uri, consumer_key, consumer_secret, "", "", "GET", timestamp, nonce, OAuthBase.SignatureTypes.HMACSHA1, "", out normalizedUrl, out normalizedRequestParameters );


            //oauth_token,oauth_token_secret取得
            HttpWebRequest webreq = (System.Net.HttpWebRequest)WebRequest.Create( OAuth.APIKey.ReqestToken + string.Format( "?{0}&oauth_signature={1}", normalizedRequestParameters, signature ) );
            webreq.Method = "GET";
            //HttpWebResponse webres = (HttpWebResponse)webreq.GetResponse();
            HttpWebResponse webres = null;

            string result = "";
            //using ( System.IO.Stream st = webres.GetResponseStream() )
            //using ( System.IO.StreamReader sr = new System.IO.StreamReader( st, Encoding.GetEncoding( 932 ) ) ) {
            //    result = sr.ReadToEnd();
            //}

            //Console.WriteLine( result );

            //正規表現でoauth_token,oauth_token_secret取得
            Match match = Regex.Match( result, @"oauth_token=(.*?)&oauth_token_secret=(.*?)&oauth_callback.*" );
            //string token = match.Groups[1].Value;
            //string tokenSecret = match.Groups[2].Value;

            string token = "17fb65da01aa217b9d11ab55d07e6f9b";
            string tokenSecret = "933fee29fd7a2215d5bc40568e42f426";


            //ブラウザからPIN確認
            //string AuthorizeURL = OAuth.APIKey.Authorize + "?" + result;
            //System.Diagnostics.Process.Start( AuthorizeURL );
            //Console.Write( "PIN:" );
            //string PIN = Console.ReadLine();

            string PIN = "8836708";
            string timeastamp = "1281594194";
            nonce = "-4553643980892898909";

            //oauth_token,oauth_token_secretを用いて再びsignature生成
            //signature = oAuth.GenerateSignature( uri, consumer_key, consumer_secret, token, tokenSecret, "POST", oAuth.GenerateTimeStamp(), oAuth.GenerateNonce(), OAuthBase.SignatureTypes.HMACSHA1, PIN, out normalizedUrl, out normalizedRequestParameters );
            uri = new Uri( OAuth.APIKey.AccessToken );
            signature = oAuth.GenerateSignature( uri, consumer_key, consumer_secret, token, tokenSecret, "POST",
                timeastamp, nonce, OAuthBase.SignatureTypes.HMACSHA1,
                PIN, out normalizedUrl, out normalizedRequestParameters );

            string request = OAuth.APIKey.AccessToken + string.Format( "?{3}&oauth_signature={0}", signature, result, PIN, normalizedRequestParameters );
            //string request = OAuth.APIKey.AccessToken + string.Format( "?oauth_verifier={0}", PIN );
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
            match = Regex.Match( result, @"oauth_token=(.*?)&oauth_token_secret=(.*?)&.*" );
            token = match.Groups[1].Value;
            tokenSecret = match.Groups[2].Value;

            Console.WriteLine( "public const string token = \"" + token + "\";" );
            Console.WriteLine( "public const string tokenSecret = \"" + tokenSecret + "\";" );

            //デスクトップ\oauth_token.txtに保存
            File.WriteAllText( Environment.GetFolderPath( Environment.SpecialFolder.Desktop ) + @"\cacoo_oauth_token.txt", result );
        }
    }
}
