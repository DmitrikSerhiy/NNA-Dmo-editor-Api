using System;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Model;
using Serilog;

namespace API.Hubs {
    [Authorize]
    public class EditorHub : Hub {

        private IBeatsRepository _beatsRepository;

        public EditorHub(IBeatsRepository beatsRepository) {
            _beatsRepository = beatsRepository ?? throw new ArgumentNullException(nameof(beatsRepository));
        }

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

        public async Task NewBeat(object beat) {
            //_beatsRepository.UpdateBeats(null);

            await Clients.Caller.SendAsync("Notify", $"{beat} received from server");
        }

    }
}
