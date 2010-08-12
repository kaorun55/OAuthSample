using System;
using OAuth;

namespace OAuthSample
{
    class Program
    {
        static void Main( string[] args )
        {
            try {
                System.Net.ServicePointManager.Expect100Continue = false;

                OAuthConsumer consumer = new OAuthConsumer( APIKey.ConsumerKey, APIKey.ConsumerSecret );
                consumer.SetTokenWithSecret( APIKey.Token, APIKey.TokenSecret );

                OAuthProvider provider = new OAuthProvider();

                string result = "";

                result = provider.RetrieveRequest( "https://cacoo.com/api/v1/account.xml", consumer );
                Console.WriteLine( "--- アカウント情報取得 ---" );
                Console.WriteLine( result );
                Console.WriteLine( "" );

                result = provider.RetrieveRequest( "https://cacoo.com/api/v1/users/kaorun55.xml", consumer );
                Console.WriteLine( "--- ユーザー情報取得 ---" );
                Console.WriteLine( result );
                Console.WriteLine( "" );

                result = provider.RetrieveRequest( "http://cacoo.com/api/v1/diagrams.xml", consumer );
                Console.WriteLine( "--- 図の一覧取得 ---" );
                Console.WriteLine( result );
                Console.WriteLine( "" );

                result = provider.RetrieveRequest( "http://cacoo.com/api/v1/diagrams/cTedXHIB8T1x1QJS.xml", consumer );
                Console.WriteLine( "--- 図の情報取得 ---" );
                Console.WriteLine( result );
                Console.WriteLine( "" );

                // バイナリの返し方を考える
                //result = provider.RetrieveRequest( "http://cacoo.com/api/v1/diagrams/cTedXHIB8T1x1QJS.png", consumer );
                //Console.WriteLine( "--- 画像取得 ---" );
                //Console.WriteLine( result );
                //Console.WriteLine( "" );
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
