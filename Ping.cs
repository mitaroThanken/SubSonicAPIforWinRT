using RestSharp;
using SubSonicAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SubSonicAPI
{
    public class Ping : ApiBase
    {
        private Ping()
        {
            throw new NotSupportedException();
        }

        public Ping(string client, Uri baseUri, string userName, string password) :
            base(client, baseUri, userName, password) { }

        public Ping(string client, Uri baseUri, string userName) :
            base(client, baseUri, userName) { }

        public async Task<IRestResponse<SubSonicResponse>> ExecuteAsync()
        {
            var request = new RestRequest();
            request.Resource = "rest/ping.view";
            request.RootElement = "subsonic-response";

            return await ExecuteAsync<SubSonicResponse>(request);
        }
    }
}
