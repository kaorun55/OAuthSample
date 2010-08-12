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
            finally {
                Console.Write( "Press enter : " );
                Console.ReadLine();
            }
        }

        private static void getAccessToken( string consumer_key, string consumer_secret )
        {
            System.Net.ServicePointManager.Expect100Continue = false;

            // リクエストトークンを取得
            OAuthProvider provider = new OAuthProvider( APIKey.AccessToken, APIKey.Authorize, APIKey.ReqestToken );
            OAuthConsumer consumer = new OAuthConsumer( consumer_key, consumer_secret );
            string AuthorizeURL = provider.RetrieveRequestToken( ref consumer );

            Trace.WriteLine( "token = " + consumer.Token );
            Trace.WriteLine( "tokenSecret = " + consumer.TokenSecret );

            //ブラウザからPIN確認
            System.Diagnostics.Process.Start( AuthorizeURL );
            Console.Write( "PIN:" );
            string pin = Console.ReadLine();
            Trace.WriteLine( "pin = " + pin );

            provider.RetrieveAccessToken( ref consumer, pin );

            Console.WriteLine( "public const string Token = \"" + consumer.Token + "\";" );
            Console.WriteLine( "public const string TokenSecret = \"" + consumer.TokenSecret + "\";" );
            Trace.WriteLine( "token = " + consumer.Token );
            Trace.WriteLine( "tokenSecret = " + consumer.TokenSecret );

            //デスクトップ\oauth_token.txtに保存
            File.WriteAllText( Environment.GetFolderPath( Environment.SpecialFolder.Desktop ) + @"\cacoo_oauth_token.txt",
                consumer.Token + ", " + consumer.TokenSecret );
        }
    }
}
