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
        /// PasswordValut に対するリソース名のプレフィクス
        /// </summary>
        protected const string PREFIX = "com.github.mitaroThanken.SubSonicAPIforWinRT_";

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
        public ApiBase(string client, Uri baseUri, string userName, string password) :
            this(client, baseUri)
        {
            var valut = new PasswordVault();
            try
            {
                credential = valut.Retrieve(PREFIX + baseUri.ToString(), userName);
                if (credential != null)
                {
                    valut.Remove(credential);
                }
            }
            catch (Exception) { }

            credential = new PasswordCredential(PREFIX + _baseUri.ToString(),
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
        public ApiBase(string client, Uri baseUri, string userName) :
            this(client, baseUri)
        {
            var valut = new PasswordVault();
            credential = valut.Retrieve(PREFIX + _baseUri.ToString(), userName);

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
        /// APIオブジェクトを取得
        /// </summary>
        /// <typeparam name="T">APIクラス</typeparam>
        /// <returns>API呼び出し用オブジェクト</returns>
        public T GetObject<T>()
            where T : ApiBase, new()
        {
            T obj = new T();
            obj._client = this._client;
            obj._baseUri = this._baseUri;
            obj.credential = this.credential;

            return obj;
        }
   
        /// <summary>
        /// RESTリクエスト
        /// </summary>
        /// <typeparam name="T">API戻りを格納する型</typeparam>
        /// <param name="req">リクエスト</param>
        /// <returns>APIからの戻り</returns>
        public Task<IRestResponse<T>> ExecuteAsync<T>(RestRequest req)
            where T : new()
        {
            var client = new RestClient();
            client.AddHandler("txt/xml", new XmlDeserializer());
            client.BaseUrl = _baseUri.ToString();
            client.Authenticator = 
                new HttpBasicAuthenticator(credential.UserName, credential.Password);
            req.AddParameter("u", credential.UserName);
            req.AddParameter("p", credential.Password);
            req.AddParameter("v", API_VERSION);
            req.AddParameter("c", _client);
            req.AddParameter("f", FORMAT);

            return client.ExecuteAsync<T>(req);
        }
    }
}
