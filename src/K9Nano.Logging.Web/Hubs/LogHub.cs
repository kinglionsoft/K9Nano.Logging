using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace K9Nano.Logging.Web.Hubs
{
    public class LogHub: Hub
    {
        public async Task Join(string add, string remove)
        {
            if (!string.IsNullOrEmpty(remove))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, remove);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, add);
        }

        public async Task Leave(string group)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        }
    }
}