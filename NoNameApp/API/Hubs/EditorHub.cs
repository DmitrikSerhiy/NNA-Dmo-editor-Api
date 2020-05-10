using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using API.DTO.Dmos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Model;
using Model.Entities;
using Serilog;
using System.Text.Json;
using API.Infrastructure.Authentication;

namespace API.Hubs {
    [Authorize]
    public class EditorHub : Hub {

        private readonly IBeatsRepository _beatsRepository;
        private readonly IMapper _mapper;
        private readonly NoNameUserManager _userManager;
        private readonly IAuthenticatedIdentityProvider _authenticatedIdentityProvider;

        public EditorHub(
            IBeatsRepository beatsRepository, 
            IMapper mapper, 
            IAuthenticatedIdentityProvider authenticatedIdentityProvider, 
            NoNameUserManager userManager) {
            _beatsRepository = beatsRepository ?? throw new ArgumentNullException(nameof(beatsRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _authenticatedIdentityProvider = authenticatedIdentityProvider ?? throw new ArgumentNullException(nameof(authenticatedIdentityProvider));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public override async Task OnConnectedAsync() {
            var userName = Context.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
            if (userName == null) {
                await OnDisconnectedAsync(new AuthenticationException("User is not authenticated [websocket]"));
                return;
            }

            var user = await _userManager.FindByNameAsync(userName.Value);
            if (user == null) {
                await OnDisconnectedAsync(new AuthenticationException("User not found [websocket]"));
                return;
            }

            _authenticatedIdentityProvider.SetAuthenticatedUser(user);
            Console.WriteLine($"{_authenticatedIdentityProvider.AuthenticatedUserEmail} just connected to the editor");
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception) {
            if (exception != null) {
                Log.Error($"Error on websoket disconnection. User: {_authenticatedIdentityProvider.AuthenticatedUserId}",
                    exception.Message);
            }

            Console.WriteLine($"{_authenticatedIdentityProvider.AuthenticatedUserEmail} disconnected from the editor");
            _authenticatedIdentityProvider.ClearAuthenticatedUserInfo();
            return base.OnDisconnectedAsync(exception);
        }

        public async Task LoadDmo(string dmoId) {
            var dmo = await _beatsRepository.LoadDmoAsync(Guid.Parse(dmoId), _authenticatedIdentityProvider.AuthenticatedUserId);
            if (dmo == null) {
                await Clients.Caller.SendAsync("LoadDmoResult");
            }
            else {
                await Clients.Caller.SendAsync("LoadDmoResult", _mapper.Map<DmoDto>(dmo));
            }
        }

        public async Task DmoUpdate(PartialDmoWithBeatsDto dmoUpdate) {
            var beats = _mapper.Map<Beat[]>(dmoUpdate.Beats);
            var dmoId = Guid.Parse(dmoUpdate.DmoId);
            var result = await _beatsRepository.UpdateBeatsAsync(JsonSerializer.Serialize(beats), dmoId);

            if (result == BeatUpdateStatus.Success) {
                await Clients.Caller.SendAsync("PartialDmoUpdateResult", $"{BeatUpdateStatus.Success}");
            }
            else {
                await Clients.Caller.SendAsync("PartialDmoUpdateResult", $"{BeatUpdateStatus.Failed}");
            }
        }

    }
}
