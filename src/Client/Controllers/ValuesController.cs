using System.Net.Http;
using System.Web.Http;
using Hubs;

namespace Client.Controllers
{
    public class ValuesController : ApiHubController<ValuesHub>
    {
        public HttpResponseMessage Post(string value)
        {
            //Make sure that the Hub method and the client method names don't clash.
            //When I renamed the actual hub method, everything began to work.
            Hub.Invoke("AddMessage", value);
            return Request.CreateResponse();
        }
    }
}