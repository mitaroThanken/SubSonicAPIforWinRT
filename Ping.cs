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
        public async Task<IRestResponse<SubSonicResponse>> ExecuteAsync()
        {
            var request = new RestRequest();
            request.Resource = "rest/ping.view";
            request.RootElement = "subsonic-response";

            return await ExecuteAsync<SubSonicResponse>(request);
        }
    }
}
