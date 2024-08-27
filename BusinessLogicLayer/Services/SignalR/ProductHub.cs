using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.SignalR
{
    public class ProductHub : Hub
    {
        public async Task UpdateProductQuantity(Guid IDOptions, int Quantity)
        {
            await Clients.Others.SendAsync("ReceiveProductQuantityUpdate", IDOptions, Quantity);
        }
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
