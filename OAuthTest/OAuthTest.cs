using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OAuth;

namespace OAuthTest
{
    /// <summary>
    /// UnitTest1 の概要の説明
    /// </summary>
    [TestClass]
    public class OAuthTest
    {
        // アクセストークン
        public const string AccessToken = "https://cacoo.com/oauth/access_token";
        public const string Authorize = "https://cacoo.com/oauth/authorize";
        public const string ReqestToken = "https://cacoo.com/oauth/request_token";

        // 通常は公開しないようにする
        public const string ConsumerKey = "OKWyYVPvnpcBfbdmrJaNWx";
        public const string ConsumerSecret = "wObshonqbOqrnhIXGwflmlEVUFiiEJqScYLmKbPYTe";

        public OAuthTest()
        {
            //
            // TODO: コンストラクタ ロジックをここに追加します
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///現在のテストの実行についての情報および機能を
        ///提供するテスト コンテキストを取得または設定します。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 追加のテスト属性
        //
        // テストを作成する際には、次の追加属性を使用できます:
        //
        // クラス内で最初のテストを実行する前に、ClassInitialize を使用してコードを実行してください
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // クラス内のテストをすべて実行したら、ClassCleanup を使用してコードを実行してください
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 各テストを実行する前に、TestInitialize を使用してコードを実行してください
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 各テストを実行した後に、TestCleanup を使用してコードを実行してください
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void PINを取得するためのトークンを取得するためのシグニチャ()
        {
            OAuth.OAuthBase oauth = new OAuth.OAuthBase();

            Uri uri = new Uri( ReqestToken );
            string timestamp = "1281614602";
            string nonce = "8715791";

            string signature = oauth.GenerateSignature( uri, ConsumerKey, ConsumerSecret, "", "", "GET",
                    timestamp, nonce, OAuthBase.SignatureTypes.HMACSHA1, "" );

            Assert.AreEqual( "FOBRl2mkgAx9tNdQeNIiIxjwhxo=", signature );
            Assert.AreEqual( "oauth_consumer_key=OKWyYVPvnpcBfbdmrJaNWx&oauth_nonce=8715791&oauth_signature_method=HMAC-SHA1&oauth_timestamp=1281614602&oauth_version=1.0",
                oauth.NormalizedRequestParameters );
        }

        [TestMethod]
        public void PINを取得後の認証のためのシグニチャ()
        {
            OAuth.OAuthBase oauth = new OAuth.OAuthBase();

            Uri uri = new Uri( AccessToken );
            string timestamp = "1281614602";
            string nonce = "8715791";
            string pin = "0011696";
            string token = "021d4561687d6c5d328ad5b491624f30";
            string tokenSecret = "552ae19dc6f2b7736db9678bb4de2f00";

            string signature = oauth.GenerateSignature( uri, ConsumerKey, ConsumerSecret, token, tokenSecret, "POST",
                    timestamp, nonce, OAuthBase.SignatureTypes.HMACSHA1, pin );

            Assert.AreEqual( "w9XSZS9loX/pyz6DtO2Q04QmDAw=", signature );
            Assert.AreEqual( "oauth_consumer_key=OKWyYVPvnpcBfbdmrJaNWx&oauth_nonce=8715791&oauth_signature_method=HMAC-SHA1&oauth_timestamp=1281614602&oauth_token=021d4561687d6c5d328ad5b491624f30&oauth_verifier=0011696&oauth_version=1.0",
                oauth.NormalizedRequestParameters );
        }

        [TestMethod]
        public void データ取得のためのシグニチャ()
        {
            OAuth.OAuthBase oauth = new OAuth.OAuthBase();

            const string Diagrams = "http://cacoo.com/api/v1/diagrams.xml";
            Uri uri = new Uri( Diagrams );

            string timestamp = "1281615317";
            string nonce = "2677625";

            string token = "f1b3a9fdb759dbd0c1818f7cdef307c0";
            string tokenSecret = "2e55a9ea2e9c286abfedadffecf15f74";

            string signature = oauth.GenerateSignature( uri, ConsumerKey, ConsumerSecret,
                token, tokenSecret,
                "POST", timestamp, nonce, OAuth.OAuthBase.SignatureTypes.HMACSHA1, "" );

            Assert.AreEqual( "YRhAaq8P+fN4DgkFhaF6x+EH1qA=", signature );
            Assert.AreEqual( "oauth_consumer_key=\"OKWyYVPvnpcBfbdmrJaNWx\", oauth_nonce=\"2677625\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"1281615317\", oauth_token=\"f1b3a9fdb759dbd0c1818f7cdef307c0\", oauth_version=\"1.0\", oauth_signature=\"YRhAaq8P%2BfN4DgkFhaF6x%2BEH1qA%3D\"",
                oauth.AuthorizationRequestParameters );
        }
    }
}
