using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OAuth
{
    /// <summary>
    /// 
    /// </summary>
    public class OAuthConsumer
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConsumerKey
        {
            private set;
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ConsumerSecret
        {
            private set;
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Token
        {
            private set;
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public string TokenSecret
        {
            private set;
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConsumerKey"></param>
        /// <param name="ConsumerSecret"></param>
        public OAuthConsumer( string ConsumerKey, string ConsumerSecret )
        {
            this.ConsumerKey = ConsumerKey;
            this.ConsumerSecret = ConsumerSecret;
            this.Token = "";
            this.TokenSecret = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="TokenSecret"></param>
        public void SetTokenWithSecret( string Token, string TokenSecret )
        {
            this.Token = Token;
            this.TokenSecret = TokenSecret;
        }
    }
}
