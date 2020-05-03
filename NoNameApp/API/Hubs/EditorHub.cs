using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace API.Hubs {
    [Authorize]
    public class EditorHub : Hub {
        public override Task OnConnectedAsync() {
            Console.WriteLine($"{Context.UserIdentifier} just connected to the editor");
            Console.WriteLine(Context.UserIdentifier);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception) {
            if (exception != null) {
                Log.Error($"Error on websoket disconnection. User: {Context.UserIdentifier}",
                    exception.Message);
            }
            Console.WriteLine($"{Context.UserIdentifier} disconnected from the editor");
            return base.OnDisconnectedAsync(exception);
        }

        public async Task NewBeat(string beat) {
            await Clients.Caller.SendAsync("Notify", $"{beat} received from server");
        }

    }
}
