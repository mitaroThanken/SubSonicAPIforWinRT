using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace SubsonicAPI
{
    public class ApiBase
    {
        /// <summary>
        /// SubSonic API バージョン
        /// </summary>
        private const string API_VERSION = "1.11.0";

        /// <summary>
        /// APIで使用するフォーマット（json）
        /// </summary>
        private const string FORMAT = "json";

        /// <summary>
        /// クライアントアプリ名
        /// </summary>
        private readonly string _client = String.Empty;

        /// <summary>
        /// PasswordValut に対するリソース名のプレフィクス
        /// </summary>
        private const string PREFIX = "com.github.mitaroThanken.SubSonicAPIforWinRT_";

        /// <summary>
        /// APIにてアクセスする先
        /// </summary>
        private readonly Uri _baseUri = null;

        /// <summary>
        /// ユーザー名とパスワード（PasswordValutから取得）
        /// </summary>
        private readonly PasswordCredential credential = null;

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
            credential = valut.Retrieve(PREFIX + baseUri.ToString(), userName);

            if (credential != null)
            {
                valut.Remove(credential);
            }

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
        /// <exception cref="NotSupportedException" />
        private ApiBase()
        {
            throw new NotSupportedException();
        }

        public Task<IRestResponse<T>> Execute<T>(RestRequest req)
            where T : new()
        {
            var client = new RestClient();
            client.BaseUrl = _baseUri.ToString();
            client.Authenticator = 
                new HttpBasicAuthenticator(credential.UserName, credential.Password);
            req.AddParameter("v", API_VERSION);
            req.AddParameter("c", _client);
            req.AddParameter("f", FORMAT);

            return client.ExecuteAsync<T>(req);
        }
    }
}
