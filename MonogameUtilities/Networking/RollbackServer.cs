
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MonogameUtilities.Networking
{
    internal class RollbackServer : Hub
    {
        public async Task SendMessage(string user, string json)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, json);
        }
    }
}
