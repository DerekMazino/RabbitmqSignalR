using Microsoft.AspNetCore.SignalR;
using RabbitSignalRClient.Models;
using System.Threading.Tasks;

namespace RabbitSignalRClient.Hubs
{
    public class StockHub: Hub
    {
        public override Task OnConnectedAsync()
        {
            return Clients.Client(Context.ConnectionId).SendAsync("SetConnectionId", Context.ConnectionId);
        }
        public async Task<string> ConnectGroup(string stocName, string connectionID)
        {
            await Groups.AddToGroupAsync(connectionID, stocName);
            return $"{connectionID} is added {stocName}";
        }
        public Task PushNotify(Stock stocData)
        {
            return Clients.Group(stocData.Name).SendAsync("ChangeStocValue", stocData);
        }
    }
}
