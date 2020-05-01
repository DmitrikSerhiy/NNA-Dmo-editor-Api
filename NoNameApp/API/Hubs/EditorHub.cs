using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs {
    [Authorize]
    public class EditorHub : Hub {
        public override Task OnConnectedAsync() {
            Console.WriteLine($"{Context.User.Identity.Name} just connected to the editor");
            Console.WriteLine(Context.UserIdentifier);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception) {
            Console.WriteLine($"{Context.User.Identity.Name} disconnected from the editor");
            return base.OnDisconnectedAsync(exception);
        }

        public async Task Send(string message) {
            

            await Clients.Caller.SendAsync("Send", message);
        }

    }
}
