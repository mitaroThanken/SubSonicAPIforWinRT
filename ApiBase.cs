using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace SubSonicAPI
{
    public class ApiBase
    {
        /// <summary>
        /// SubSonic API バージョン
        /// </summary>
        protected const string API_VERSION = "1.11.0";

        /// <summary>
        /// APIで使用するフォーマット（json）
        /// </summary>
        protected const string FORMAT = "json";

        /// <summary>
        /// クライアントアプリ名
        /// </summary>
        protected string _client = String.Empty;

        /// <summary>
        /// APIにてアクセスする先
        /// </summary>
        protected Uri _baseUri = null;

        /// <summary>
        /// ユーザー名とパスワード（PasswordValutから取得）
        /// </summary>
        protected PasswordCredential credential = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <remarks>
        /// PasswordValutに対する追加・更新
        /// </remarks>
        /// <param name="client">クライアントアプリ名</param>
        /// <param name="baseUri">ベースURI</param>
        /// <param name="userName">ユーザー名</param>
        /// <param name="password">パスワード</param>
        protected ApiBase(string client, Uri baseUri, string userName, string password) :
            this(client, baseUri)
        {
            var valut = new PasswordVault();
            try
            {
                credential = valut.Retrieve(baseUri.ToString(), userName);
                if (credential != null)
                {
                    valut.Remove(credential);
                }
            }
            catch (Exception) { }

            credential = new PasswordCredential(baseUri.ToString(),
                                                userName, password);
            valut.Add(credential);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <remarks>
        /// PasswordValutからの参照
        /// </remarks>
        /// <param name="client">クライアントアプリ名</param>
        /// <param name="baseUri">ベースURI</param>
        /// <param name="userName">ユーザー名</param>
        /// <exception cref="ArgumentException"/>
        protected ApiBase(string client, Uri baseUri, string userName) :
            this(client, baseUri)
        {
            var valut = new PasswordVault();
            credential = valut.Retrieve(baseUri.ToString(), userName);

            if (null == credential)
            {
                // パスワードがまだ登録されていない。
                throw new ArgumentException();
            }
        } 
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <remarks>
        /// 公開している２つのコンストラクタに共通する事前処理
        /// </remarks>
        /// <param name="client">クライアントアプリ名</param>
        /// <param name="baseUri">ベースURI</param>
        private ApiBase(string client, Uri baseUri)
        {
            _client = client;
            _baseUri = baseUri;
        }

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        /// <remarks>
        /// 使用不可
        /// </remarks>
        protected ApiBase() { }

        /// <summary>
        /// RESTリクエスト
        /// </summary>
        /// <typeparam name="T">API戻りを格納する型</typeparam>
        /// <param name="req">リクエスト</param>
        /// <returns>APIからの戻り</returns>
        protected Task<IRestResponse<T>> ExecuteAsync<T>(RestRequest req)
            where T : new()
        {
            var client = new RestClient();
            client.AddHandler("txt/xml", new XmlDeserializer());
            client.BaseUrl = _baseUri.ToString();
            client.Authenticator =
                new SimpleAuthenticator("u", credential.UserName, "p", credential.Password);
            req.AddParameter("v", API_VERSION);
            req.AddParameter("c", _client);
            req.AddParameter("f", FORMAT);

            return client.ExecuteAsync<T>(req);
        }
    }
}
