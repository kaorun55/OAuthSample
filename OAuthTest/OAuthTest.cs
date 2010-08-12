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

        public const string Token = "17fb65da01aa217b9d11ab55d07e6f9b";
        public const string TokenSecret = "933fee29fd7a2215d5bc40568e42f426";

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
        public void PINを取得後の認証()
        {
            OAuth.OAuthBase oauth = new OAuth.OAuthBase();

            Uri uri = new Uri( AccessToken );
            string timestamp = "1281594194";
            string nonce = "-4553643980892898909";
            string pin = "8836708";
            string normalizedUrl, normalizedRequestParameters, s;
            string signature = oauth.GenerateSignature( uri, ConsumerKey, ConsumerSecret, Token, TokenSecret, "POST",
                    timestamp, nonce, OAuthBase.SignatureTypes.HMACSHA1, pin,
                    out normalizedUrl, out normalizedRequestParameters, out s );

            Assert.AreEqual( "XIhBEU281BfkWcvgU20fKlrYCIk=", signature );
        }
    }
}
