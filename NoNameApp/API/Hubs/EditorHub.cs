using System;
using System.Threading.Tasks;
using API.DTO.Dmos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Model;
using Model.Entities;
using Serilog;

namespace API.Hubs {
    [Authorize]
    public class EditorHub : Hub {

        private readonly IBeatsRepository _beatsRepository;
        private readonly IMapper _mapper;

        public EditorHub(
            IBeatsRepository beatsRepository, 
            IMapper mapper) {
            _beatsRepository = beatsRepository ?? throw new ArgumentNullException(nameof(beatsRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

        public async Task DmoUpdate(PartialDmoWithBeatsDto dmoUpdate) {
            var beats = _mapper.Map<Beat[]>(dmoUpdate.Beats);
            var dmoId = Guid.Parse(dmoUpdate.DmoId);
            _beatsRepository.UpdateBeats(beats, dmoId);

            await Clients.Caller.SendAsync("UpdateNotify", $"Dmo {dmoUpdate.DmoId} saved");
        }

    }
}
