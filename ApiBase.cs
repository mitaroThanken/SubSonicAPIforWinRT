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
        /// 資格情報
        /// </summary>
        protected PasswordCredential _credential;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <remarks>
        /// PasswordValutに対する追加・更新
        /// </remarks>
        /// <param name="client">クライアントアプリ名</param>
        /// <param name="credential">資格情報</param>
        protected ApiBase(string client, PasswordCredential credential)
        {
            _client = client;
            _credential = credential;
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
            client.BaseUrl = _credential.Resource;
            client.Authenticator =
                new SimpleAuthenticator("u", _credential.UserName, "p", _credential.Password);
            req.AddParameter("v", API_VERSION);
            req.AddParameter("c", _client);
            req.AddParameter("f", FORMAT);

            return client.ExecuteAsync<T>(req);
        }
    }
}
