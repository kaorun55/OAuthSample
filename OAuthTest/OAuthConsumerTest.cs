using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OAuthTest
{
    /// <summary>
    /// OAuthConsumerTest の概要の説明
    /// </summary>
    [TestClass]
    public class OAuthConsumerTest
    {
        // アクセストークン
        public const string AccessToken = "https://cacoo.com/oauth/access_token";
        public const string Authorize = "https://cacoo.com/oauth/authorize";
        public const string ReqestToken = "https://cacoo.com/oauth/request_token";

        public OAuthConsumerTest()
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
        public void コンストラクタでコンシューマキーを設定する()
        {
            OAuth.OAuthConsumer consumer = new OAuth.OAuthConsumer( "aaa", "bbb" );
            Assert.AreEqual( "aaa", consumer.ConsumerKey );
            Assert.AreEqual( "bbb", consumer.ConsumerSecret );
            Assert.AreEqual( "", consumer.Token );
            Assert.AreEqual( "", consumer.TokenSecret );
        }

        [TestMethod]
        public void トークンキーを設定する()
        {
            OAuth.OAuthConsumer consumer = new OAuth.OAuthConsumer( "aaa", "bbb" );
            consumer.SetTokenWithSecret( "ccc", "ddd" );
            Assert.AreEqual( "aaa", consumer.ConsumerKey );
            Assert.AreEqual( "bbb", consumer.ConsumerSecret );
            Assert.AreEqual( "ccc", consumer.Token );
            Assert.AreEqual( "ddd", consumer.TokenSecret );
        }
    }
}
