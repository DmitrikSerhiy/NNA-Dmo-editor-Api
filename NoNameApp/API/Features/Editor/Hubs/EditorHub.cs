using System;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;
using API.Features.Account.Services;
using API.Features.Editor.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Model;
using Model.DTOs.Dmos;
using Model.Entities;
using Model.Enums;
using Model.Interfaces;
using Model.Interfaces.Repositories;
using Serilog;

namespace API.Features.Editor.Hubs {
    [Authorize]
    public class EditorHub : Hub {

        private readonly IEditorRepository _editorRepository;
        private readonly IEditorService _editorService;
        private readonly IMapper _mapper;
        private readonly NnaUserManager _userManager;

        public EditorHub(
            IEditorRepository editorRepository, 
            IMapper mapper,
            NnaUserManager userManager, 
            IEditorService editorService) {
            _editorRepository = editorRepository ?? throw new ArgumentNullException(nameof(editorRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); 
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _editorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
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

            return await _editorService.CreateAndLoadAsync(dmoDto, Context.GetCurrentUserId().GetValueOrDefault());
        }

        public async Task<ShortDmoDto> LoadDmo(string dmoId) {
            if (!Context.ContainsUser()) {
                return null;
            }
            var dmo = await _editorRepository
                .LoadShortDmoAsync(Guid.Parse(dmoId), Context.GetCurrentUserId().GetValueOrDefault());
            return dmo == null ? null : _mapper.Map<ShortDmoDto>(dmo);
        }

        public async Task<ShortDmoDto> UpdateDmoInfo(ShortDmoDto dmoDto) {
            if (!Context.ContainsUser()) {
                return null;
            }

            var updatedDmo = await _editorRepository
                .EditDmoAsync(_mapper.Map<Dmo>(dmoDto), Context.GetCurrentUserId().GetValueOrDefault());
            return updatedDmo == null ? null : _mapper.Map<ShortDmoDto>(updatedDmo);
        }




        public async Task DmoUpdate(ShortDmoWithBeatsDto dmoUpdate) {
            var dmoId = Guid.Parse(dmoUpdate.Id);
            var result = await _editorRepository.UpdateBeatsAsync(
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
