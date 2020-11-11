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
using API.Hubs.Extensions;
using API.Hubs.Helpers;
using API.Infrastructure.Authentication;
using Model.Enums;

namespace API.Hubs {
    [Authorize]
    public class EditorHub : Hub {

        private readonly IBeatsRepository _beatsRepository;
        private readonly IMapper _mapper;
        private readonly NoNameUserManager _userManager;

        public EditorHub(
            IBeatsRepository beatsRepository, 
            IMapper mapper,
            NoNameUserManager userManager) {
            //temp
            Log.Information($"from constructor: {beatsRepository == null} {mapper == null} {userManager == null}");
            _beatsRepository = beatsRepository ?? throw new ArgumentNullException(nameof(beatsRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); 
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public override async Task OnConnectedAsync() {
            var userName = Context.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
            //temp
            Log.Information($"from onConnected: {userName.Value}");
            if (userName == null) {
                await OnDisconnectedAsync(new AuthenticationException("User is not authenticated [websocket]"));
                return;
            }

            var user = await _userManager.FindByNameAsync(userName.Value);
            if (user == null) {
                await OnDisconnectedAsync(new AuthenticationException("User not found [websocket]"));
                return;
            }
            Context.SaveUser(user);
            Console.WriteLine($"{Context.GetCurrentUserEmail()} just connected to the editor");
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception) {
            if (exception != null) {
                Log.Error(exception, $"Error on websocket disconnection. User: {Context.GetCurrentUserId()}. Error: {exception.Message}");
            }

            Console.WriteLine($"{Context.GetCurrentUserEmail()} disconnected from the editor");
            Context.RemoveUser();
            return base.OnDisconnectedAsync(exception);
        }

        public async Task<ShortDmoDto> CreateDmo(ShortDmoDto dmoDto) {
            if (!Context.ContainsUser()) {
                return null;
            }

            var createdDmo = await _beatsRepository
                .CreateDmoAsync(_mapper.Map<Dmo>(dmoDto), Context.GetCurrentUserId().GetValueOrDefault());
            return createdDmo == null ? null : _mapper.Map<ShortDmoDto>(createdDmo);
        }

        public async Task<ShortDmoDto> LoadDmo(string dmoId) {
            if (!Context.ContainsUser()) {
                return null;
            }
            var dmo = await _beatsRepository
                .LoadDmoAsync(Guid.Parse(dmoId), Context.GetCurrentUserId().GetValueOrDefault());
            return dmo == null ? null : _mapper.Map<ShortDmoDto>(dmo);
        }

        public async Task<ShortDmoDto> UpdateDmoInfo(ShortDmoDto dmoDto) {
            if (!Context.ContainsUser()) {
                return null;
            }

            var updatedDmo = await _beatsRepository
                .EditDmoAsync(_mapper.Map<Dmo>(dmoDto), Context.GetCurrentUserId().GetValueOrDefault());
            return updatedDmo == null ? null : _mapper.Map<ShortDmoDto>(updatedDmo);
        }




        public async Task DmoUpdate(ShortDmoWithBeatsDto dmoUpdate) {
            var dmoId = Guid.Parse(dmoUpdate.Id);
            var result = await _beatsRepository.UpdateBeatsAsync(
                JsonSerializer.Serialize(BeatsIdGenerator.GenerateMissingBeatsIds(dmoUpdate.Beats)), dmoId);

            if (result == BeatUpdateStatus.Success) {
                await Clients.Caller.SendAsync("PartialDmoUpdateResult", $"{BeatUpdateStatus.Success}");
            }
            else {
                await Clients.Caller.SendAsync("PartialDmoUpdateResult", $"{BeatUpdateStatus.Failed}");
            }
        }
    }
}
