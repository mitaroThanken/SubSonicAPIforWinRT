using RestSharp;
using RestSharp.Deserializers;
using SubSonicAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SubSonicAPI
{
    [DeserializeAs(Name="subsonic-response")]
    public class SubSonicResponse
    {
        public string status { get; set; }
        public string version { get; set; }

        public Error error { get; set; }
   }
}
