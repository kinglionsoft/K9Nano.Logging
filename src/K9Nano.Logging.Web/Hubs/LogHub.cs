using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace K9Nano.Logging.Web.Hubs
{
    public class LogHub: Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}