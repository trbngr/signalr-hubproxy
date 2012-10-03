using System;
using System.Web.Mvc;
using SignalR;
using SignalR.Hubs;

namespace Host.Controllers
{
    public abstract class HubController<THub> : Controller where THub : IHub
    {
        private readonly Lazy<IHubContext> _hub =
            new Lazy<IHubContext>(() => GlobalHost.ConnectionManager.GetHubContext<THub>());

        protected IHubContext Hub
        {
            get { return _hub.Value; }
        }
    }
}