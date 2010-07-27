﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;

namespace TwitterOAuthSample
{
    class OAuthTwitter
    {
        //APIのURLとパラメータでAPIにアクセス
        public string PostAPI( string APIURL, Dictionary<string, string> query )
        {
            string result, queryString;
            result = queryString = string.Empty;

            string signature = GenerateSignature( APIURL, "POST", query, OAuth.APIKey.ConsumerSecret, OAuth.APIKey.tokenSecret, out queryString );

            //生成後のsignatureは小文字でパーセントエンコード
            string postString = queryString + string.Format( "&oauth_signature={0}", UrlEncodeSmall( signature ) );

            byte[] data = Encoding.ASCII.GetBytes( postString );
            WebRequest req = WebRequest.Create( APIURL );
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = data.Length;
            Stream reqStream = req.GetRequestStream();
            reqStream.Write( data, 0, data.Length );
            reqStream.Close();
            try {
                WebResponse res = req.GetResponse();
                Stream resStream = res.GetResponseStream();
                StreamReader reader = new StreamReader( resStream, Encoding.GetEncoding( 932 ) );
                result = reader.ReadToEnd();
                reader.Close();
                resStream.Close();
            }
            catch ( WebException ex ) {
                if ( ex.Status == WebExceptionStatus.ProtocolError ) {
                    if ( ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized ) {
                        /*401 Unauthorized                    
                        *認証失敗*/
                        return "401 Unauthorized";
                    }
                    else if ( ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.BadRequest ) {
                        /*400 Bad Request                    
                         *リクエストが不正*/
                        return "400 Bad Request";
                    }
                    else if ( ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Forbidden ) {
                        /*403 Forbidden                    
                        *使用不可*/
                        return "403 Forbidden";
                    }
                }
                return ex.Message;
            }
            return result;
        }

        //APIのURL,HttpMethod(POST/GET),パラメータ,consumer_secret,token_secretでsignature生成
        private string GenerateSignature( string url, string httpMethod, Dictionary<string, string> query, string consumer_secret, string token_secret, out string conectedQuery )
        {
            //SortedDictionaryでパラメータをkey順でソート
            SortedDictionary<string, string> sortedParams;
            if ( query==null )
                sortedParams = new SortedDictionary<string, string>();
            else
                sortedParams = new SortedDictionary<string, string>( query );

            string timestamp = GenerateTimestamp();
            string nonce = GenerateNonce();

            sortedParams["oauth_consumer_key"] = OAuth.APIKey.ConsumerKey;
            sortedParams["oauth_token"] = OAuth.APIKey.token;
            sortedParams["oauth_version"] = "1.0";
            sortedParams["oauth_timestamp"] = timestamp;
            sortedParams["oauth_nonce"] = nonce;
            sortedParams["oauth_signature_method"] = "HMAC-SHA1";

            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach ( var p in sortedParams ) {
                if ( first ) {
                    sb.Append( p.Key + "=" + p.Value );
                    first = false;
                }
                else
                    sb.Append( @"&" + p.Key + "=" + p.Value );
            }
            conectedQuery = sb.ToString();
            string signatureBace = string.Format( @"{0}&{1}&{2}", httpMethod, UrlEncode( url ), UrlEncode( sb.ToString() ) );

            //consumer_secretとtoken_secretを鍵にしてハッシュ値を求める
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = Encoding.ASCII.GetBytes( string.Format( "{0}&{1}", UrlEncode( consumer_secret ), UrlEncode( token_secret ) ) );
            byte[] dataBuffer = System.Text.Encoding.ASCII.GetBytes( signatureBace );
            byte[] hashBytes = hmacsha1.ComputeHash( dataBuffer );

            return Convert.ToBase64String( hashBytes );
        }

        private string GenerateNonce()
        {
            string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder result = new StringBuilder( 8 );
            Random random = new Random();
            for ( int i = 0; i < 8; ++i )
                result.Append( letters[random.Next( letters.Length )] );
            return result.ToString();
        }

        private string GenerateTimestamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime( 1970, 1, 1, 0, 0, 0, 0 );
            return Convert.ToInt64( ts.TotalSeconds ).ToString();
        }

        public string UrlEncode( string value )
        {
            string unreserved = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            StringBuilder result = new StringBuilder();
            byte[] data = Encoding.UTF8.GetBytes( value );
            foreach ( byte b in data ) {
                if ( b < 0x80 && unreserved.IndexOf( (char)b ) != -1 )
                    result.Append( (char)b );
                else
                    result.Append( '%' + String.Format( "{0:X2}", (int)b ) );
            }
            return result.ToString();
        }
        public string UrlEncodeSmall( string value )
        {
            string unreserved = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            StringBuilder result = new StringBuilder();
            byte[] data = Encoding.UTF8.GetBytes( value );
            foreach ( byte b in data ) {
                if ( b < 0x80 && unreserved.IndexOf( (char)b ) != -1 )
                    result.Append( (char)b );
                else
                    result.Append( '%' + String.Format( "{0:x2}", (int)b ) );
            }
            return result.ToString();
        }

    }
    class Program
    {
        static void Main( string[] args )
        {
            System.Net.ServicePointManager.Expect100Continue = false;

            Dictionary<string, string> query = new Dictionary<string, string>();

            //TwitterAPIを利用してpost
            OAuthTwitter oauthTwitter = new OAuthTwitter();
            query["status"] = oauthTwitter.UrlEncode( "API post" );
            Console.WriteLine( oauthTwitter.PostAPI( "http://twitter.com/statuses/update.xml", query ) );
        }
    }
}
