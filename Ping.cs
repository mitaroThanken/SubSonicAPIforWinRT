using RestSharp;
using SubSonicAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace SubSonicAPI
{
    public class Ping : ApiBase
    {
        private Ping()
        {
            throw new NotSupportedException();
        }

        public Ping(string client, PasswordCredential credential) :
            base(client, credential) { }

        public async Task<IRestResponse<SubSonicResponse>> ExecuteAsync()
        {
            var request = new RestRequest();
            request.Resource = "rest/ping.view";
            request.RootElement = "subsonic-response";

            var response = await ExecuteAsync<SubSonicResponse>(request);
            
            Debug.WriteLine("StatusCode: {0}", response.StatusCode);
            if (null != response.Data)
            {
                Debug.WriteLine("Status: {0}", response.Data.status);
                if (null != response.Data.error)
                {
                    Debug.WriteLine("ErrorCode: {0}", response.Data.error.code);
                    Debug.WriteLine("Message: {0}", response.Data.error.message);
                }
            }

            return response;
        }
    }
}
