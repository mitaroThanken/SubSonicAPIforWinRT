using RestSharp;
using SubsonicAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SubSonicAPI
{
    public class Ping : SubSonicResponce
    {
        public async Task<IRestResponse<Ping>> ExecuteAsync()
        {
            var request = new RestRequest();
            request.Resource = "rest/ping.view";
            request.RootElement = "subsonic-response";

            return await ExecuteAsync<Ping>(request);
        }
    }
}
