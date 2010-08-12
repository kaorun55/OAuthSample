using System;
using System.IO;
using OAuth;

namespace TwitterOAuthGetAccessToken
{
    class Program
    {
        static void Main( string[] args )
        {
            try {
                System.Net.ServicePointManager.Expect100Continue = false;

                // リクエストトークンを取得
                OAuthProvider provider = new OAuthProvider( APIKey.AccessToken, APIKey.Authorize, APIKey.ReqestToken );
                OAuthConsumer consumer = new OAuthConsumer( OAuth.APIKey.ConsumerKey, OAuth.APIKey.ConsumerSecret );
                string authorizeURL = provider.RetrieveRequestToken( ref consumer );

                //ブラウザからPIN確認
                System.Diagnostics.Process.Start( authorizeURL );
                Console.Write( "PIN:" );
                string pin = Console.ReadLine();

                // アクセストークンを取得
                provider.RetrieveAccessToken( ref consumer, pin );

                Console.WriteLine( "public const string Token = \"" + consumer.Token + "\";" );
                Console.WriteLine( "public const string TokenSecret = \"" + consumer.TokenSecret + "\";" );

                //デスクトップ\oauth_token.txtに保存
                File.WriteAllText( Environment.GetFolderPath( Environment.SpecialFolder.Desktop ) + @"\cacoo_oauth_token.txt",
                    consumer.Token + ", " + consumer.TokenSecret );
            }
            catch ( Exception ex ) {
                Console.WriteLine( ex.Message );
            }
            finally {
                Console.Write( "Press enter : " );
                Console.ReadLine();
            }
        }
    }
}
