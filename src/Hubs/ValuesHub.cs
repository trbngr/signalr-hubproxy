using SignalR.Hubs;

namespace Hubs
{
    [HubName("values")]
    public class ValuesHub : Hub
    {
        public void AddMessage(string value)
        {
            Clients.add(value);
        }
    }
}