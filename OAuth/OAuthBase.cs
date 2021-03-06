using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace OAuth
{
    /// <summary>
    /// OAuthの認証
    /// </summary>
    public class OAuthBase
    {

        /// <summary>
        /// Provides a predefined set of algorithms that are supported officially by the protocol
        /// </summary>
        public enum SignatureTypes
        {
            HMACSHA1,
            PLAINTEXT,
            RSASHA1
        }

        /// <summary>
        /// Provides an internal structure to sort the query parameter
        /// </summary>
        protected class QueryParameter
        {
            private string name = null;
            private string value = null;

            public QueryParameter( string name, string value )
            {
                this.name = name;
                this.value = value;
            }

            public string Name
            {
                get
                {
                    return name;
                }
            }

            public string Value
            {
                get
                {
                    return value;
                }
            }
        }

        /// <summary>
        /// Comparer class used to perform the sorting of the query parameters
        /// </summary>
        protected class QueryParameterComparer : IComparer<QueryParameter>
        {

            #region IComparer<QueryParameter> Members

            public int Compare( QueryParameter x, QueryParameter y )
            {
                if ( x.Name == y.Name ) {
                    return string.Compare( x.Value, y.Value );
                }
                else {
                    return string.Compare( x.Name, y.Name );
                }
            }

            #endregion
        }

        protected const string OAuthVersion = "1.0";
        protected const string OAuthParameterPrefix = "oauth_";

        //
        // List of know and used oauth parameters' names
        //        
        protected const string OAuthConsumerKeyKey = "oauth_consumer_key";
        protected const string OAuthCallbackKey = "oauth_callback";
        protected const string OAuthVersionKey = "oauth_version";
        protected const string OAuthSignatureMethodKey = "oauth_signature_method";
        protected const string OAuthSignatureKey = "oauth_signature";
        protected const string OAuthTimestampKey = "oauth_timestamp";
        protected const string OAuthNonceKey = "oauth_nonce";
        protected const string OAuthTokenKey = "oauth_token";
        protected const string OAuthTokenSecretKey = "oauth_token_secret";
        protected const string OAuthVerifier = "oauth_verifier";

        protected const string HMACSHA1SignatureType = "HMAC-SHA1";
        protected const string PlainTextSignatureType = "PLAINTEXT";
        protected const string RSASHA1SignatureType = "RSA-SHA1";

        protected Random random = new Random();

        protected string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        /// <summary>
        /// 
        /// </summary>
        public string NormalizedUrl
        {
            private set;
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public string NormalizedRequestParameters
        {
            private set;
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public string AuthorizationRequestParameters
        {
            private set;
            get;
        }

        /// <summary>
        /// Helper function to compute a hash value
        /// </summary>
        /// <param name="hashAlgorithm">The hashing algoirhtm used. If that algorithm needs some initialization, like HMAC and its derivatives, they should be initialized prior to passing it to this function</param>
        /// <param name="data">The data to hash</param>
        /// <returns>a Base64 string of the hash value</returns>
        private string ComputeHash( HashAlgorithm hashAlgorithm, string data )
        {
            if ( hashAlgorithm == null ) {
                throw new ArgumentNullException( "hashAlgorithm" );
            }

            if ( string.IsNullOrEmpty( data ) ) {
                throw new ArgumentNullException( "data" );
            }

            byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes( data );
            byte[] hashBytes = hashAlgorithm.ComputeHash( dataBuffer );

            return Convert.ToBase64String( hashBytes );
        }

        /// <summary>
        /// Internal function to cut out all non oauth query string parameters (all parameters not begining with "oauth_")
        /// </summary>
        /// <param name="parameters">The query string part of the Url</param>
        /// <returns>A list of QueryParameter each containing the parameter name and value</returns>
        private List<QueryParameter> GetQueryParameters( string parameters )
        {
            if ( parameters.StartsWith( "?" ) ) {
                parameters = parameters.Remove( 0, 1 );
            }

            List<QueryParameter> result = new List<QueryParameter>();

            if ( !string.IsNullOrEmpty( parameters ) ) {
                string[] p = parameters.Split( '&' );
                foreach ( string s in p ) {
                    if ( !string.IsNullOrEmpty( s ) && !s.StartsWith( OAuthParameterPrefix ) ) {
                        if ( s.IndexOf( '=' ) > -1 ) {
                            string[] temp = s.Split( '=' );
                            result.Add( new QueryParameter( temp[0], temp[1] ) );
                        }
                        else {
                            result.Add( new QueryParameter( s, string.Empty ) );
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower case.
        /// While this is not a problem with the percent encoding spec, it is used in upper case throughout OAuth
        /// </summary>
        /// <param name="value">The value to Url encode</param>
        /// <returns>Returns a Url encoded string</returns>
        protected string UrlEncode( string value )
        {
            StringBuilder result = new StringBuilder();

            foreach ( char symbol in value ) {
                if ( unreservedChars.IndexOf( symbol ) != -1 ) {
                    result.Append( symbol );
                }
                else {
                    result.Append( '%' + String.Format( "{0:X2}", (int)symbol ) );
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Normalizes the request parameters according to the spec
        /// </summary>
        /// <param name="parameters">The list of parameters already sorted</param>
        /// <returns>a string representing the normalized parameters</returns>
        protected string GenerateNormalizeRequestParameters( IList<QueryParameter> parameters )
        {
            StringBuilder sb = new StringBuilder();
            QueryParameter p = null;
            for ( int i = 0; i < parameters.Count; i++ ) {
                p = parameters[i];
                sb.AppendFormat( "{0}={1}", p.Name, p.Value );

                if ( i < parameters.Count - 1 ) {
                    sb.Append( "&" );
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generate the signature base that is used to produce the signature
        /// </summary>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>        
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="signatureType">The signature type. To use the default values use <see cref="OAuthBase.SignatureTypes">OAuthBase.SignatureTypes</see>.</param>
        /// <returns>The signature base</returns>
        public string GenerateSignatureBase( Uri url, OAuthConsumer consumer, string httpMethod, string signatureType, string pin )
        {
            if ( string.IsNullOrEmpty( consumer.ConsumerKey ) ) {
                throw new ArgumentNullException( "consumerKey" );
            }

            if ( string.IsNullOrEmpty( httpMethod ) ) {
                throw new ArgumentNullException( "httpMethod" );
            }

            if ( string.IsNullOrEmpty( signatureType ) ) {
                throw new ArgumentNullException( "signatureType" );
            }

            List<QueryParameter> parameters = GenerateParameters( url, consumer, signatureType, pin );

            NormalizedUrl = GenerateNormalizedUrl( url );
            NormalizedRequestParameters = GenerateNormalizeRequestParameters( parameters );
            AuthorizationRequestParameters = GenerateAuthorizationRequestParameters( parameters );

            return GenerateSignatureBase( httpMethod );
        }

        /// <summary>
        /// NormalizedUrlの生成
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GenerateNormalizedUrl( Uri url )
        {
            string NormalizedUrl = string.Format( "{0}://{1}", url.Scheme, url.Host );
            if ( !((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)) ) {
                NormalizedUrl += ":" + url.Port;
            }

            return NormalizedUrl + url.AbsolutePath;
        }

        /// <summary>
        /// パラメータの作成
        /// </summary>
        /// <param name="url"></param>
        /// <param name="consumer"></param>
        /// <param name="timeStamp"></param>
        /// <param name="nonce"></param>
        /// <param name="signatureType"></param>
        /// <param name="pin"></param>
        /// <returns></returns>
        private List<QueryParameter> GenerateParameters( Uri url, OAuthConsumer consumer, string signatureType, string pin )
        {
            List<QueryParameter> parameters = GetQueryParameters( url.Query );

            parameters.Add( new QueryParameter( OAuthConsumerKeyKey, consumer.ConsumerKey ) );
            parameters.Add( new QueryParameter( OAuthVersionKey, OAuthVersion ) );
            parameters.Add( new QueryParameter( OAuthSignatureMethodKey, signatureType ) );
            parameters.Add( new QueryParameter( OAuthTimestampKey, GenerateTimeStamp() ) );
            parameters.Add( new QueryParameter( OAuthNonceKey, GenerateNonce() ) );

            if ( !string.IsNullOrEmpty( consumer.Token ) ) {
                parameters.Add( new QueryParameter( OAuthTokenKey, consumer.Token ) );
            }

            if ( !string.IsNullOrEmpty( pin ) ) {
                parameters.Add( new QueryParameter( OAuthVerifier, pin ) );
            }

            parameters.Sort( new QueryParameterComparer() );
            return parameters;
        }

        /// <summary>
        /// シグニチャのベースを生成する
        /// </summary>
        /// <param name="httpMethod"></param>
        /// <returns></returns>
        private string GenerateSignatureBase( string httpMethod )
        {
            StringBuilder signatureBase = new StringBuilder();
            signatureBase.AppendFormat( "{0}&", httpMethod.ToUpper() );
            signatureBase.AppendFormat( "{0}&", UrlEncode( NormalizedUrl ) );
            signatureBase.AppendFormat( "{0}", UrlEncode( NormalizedRequestParameters ) );

            return signatureBase.ToString();
        }

        /// <summary>
        /// Authorization 用パラメータの生成
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private string GenerateAuthorizationRequestParameters( List<QueryParameter> parameters )
        {
            StringBuilder sb = new StringBuilder();
            QueryParameter p = null;
            for ( int i = 0; i < parameters.Count; i++ ) {
                p = parameters[i];
                sb.AppendFormat( "{0}=\"{1}\", ", p.Name, p.Value );
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generate the signature value based on the given signature base and hash algorithm
        /// </summary>
        /// <param name="signatureBase">The signature based as produced by the GenerateSignatureBase method or by any other means</param>
        /// <param name="hash">The hash algorithm used to perform the hashing. If the hashing algorithm requires initialization or a key it should be set prior to calling this method</param>
        /// <returns>A base64 string of the hash value</returns>
        public string GenerateSignatureUsingHash( string signatureBase, HashAlgorithm hash )
        {
            return ComputeHash( hash, signatureBase );
        }

        /// <summary>
        /// Generates a signature using the HMAC-SHA1 algorithm
        /// </summary>		
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <returns>A base64 string of the hash value</returns>
        public string GenerateSignature( Uri url, OAuthConsumer consumer, string httpMethod, string pin  )
        {
            return GenerateSignature( url, consumer, httpMethod, SignatureTypes.HMACSHA1, pin );
        }

        /// <summary>
        /// Generates a signature using the specified signatureType 
        /// </summary>		
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="signatureType">The type of signature to use</param>
        /// <returns>A base64 string of the hash value</returns>
        public string GenerateSignature( Uri url, OAuthConsumer consumer, string httpMethod, SignatureTypes signatureType, string pin )
        {
            switch ( signatureType ) {
            case SignatureTypes.PLAINTEXT:
                return HttpUtility.UrlEncode( string.Format( "{0}&{1}", consumer.ConsumerSecret, consumer.TokenSecret ) );
            case SignatureTypes.HMACSHA1:
                string signatureBase = GenerateSignatureBase( url, consumer, httpMethod, HMACSHA1SignatureType, pin );
                string signature = GenerateSignature( consumer, signatureBase );

                // 認証パラメータ用にシグニチャにを付加する
                AuthorizationRequestParameters += string.Format( "oauth_signature=\"{0}\"", UrlEncode( signature ) );

                return signature;
            case SignatureTypes.RSASHA1:
                throw new NotImplementedException();
            default:
                throw new ArgumentException( "Unknown signature type", "signatureType" );
            }
        }

        /// <summary>
        /// シグニチャを作成する
        /// </summary>
        /// <param name="consumer"></param>
        /// <param name="signatureBase"></param>
        /// <returns></returns>
        private string GenerateSignature( OAuthConsumer consumer, string signatureBase )
        {
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = Encoding.UTF8.GetBytes( GenerateMacKey( consumer ) );
            return GenerateSignatureUsingHash( signatureBase, hmacsha1 );
        }

        /// <summary>
        /// HMACSHA1のためのキーを作成する
        /// </summary>
        /// <param name="consumer"></param>
        /// <returns></returns>
        private string GenerateMacKey( OAuthConsumer consumer )
        {
            return string.Format( "{0}&{1}", UrlEncode( consumer.ConsumerSecret ), consumer.TokenSecret );
        }

        /// <summary>
        /// Generate the timestamp for the signature        
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            TimeSpan ts = DateTime.UtcNow - new DateTime( 1970, 1, 1, 0, 0, 0, 0 );
            return Convert.ToInt64( ts.TotalSeconds ).ToString();
        }

        /// <summary>
        /// Generate a nonce
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateNonce()
        {
            // Just a simple implementation of a random number between 123400 and 9999999
            return random.Next( 123400, 9999999 ).ToString();
        }
    }
}
