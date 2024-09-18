using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.SignalR
{
    public class VoucherHub : Hub
    {
        public async Task UpdateVoucherStatus(string voucherId, int newStatus)
        {
            await Clients.All.SendAsync("ReceiveVoucherStatusUpdate", voucherId, newStatus);
        }
    }
}
