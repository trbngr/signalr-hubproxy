using System.Web.Http;
using SignalR.Client.Hubs;
using SignalR.Hubs;

namespace Client.Controllers
{
    public abstract class ApiHubController<THub> : ApiController where THub : IHub
    {
        private readonly HubProxies _proxies;

        protected ApiHubController()
        {
            _proxies = HubProxies.Instance;
        }

        protected IHubProxy Hub
        {
            get { return _proxies.Get<THub>(); }
        }
    }
}