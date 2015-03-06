using RestSharp;
using RestSharp.Deserializers;
using SubsonicAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SubSonicAPI
{
    [DeserializeAs(Name="subsonic-responce")]
    public class SubSonicResponce : ApiBase
    {
        public string status { get; set; }
        public string version { get; set; }

        public Error error { get; set; }
   }
}
