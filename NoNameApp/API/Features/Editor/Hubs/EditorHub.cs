using API.Features.Account.Services;
using API.Features.Editor.Services;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Model.DTOs.Dmos;
using Model.Entities;
using Model.Enums;
using Model.Interfaces;
using Model.Interfaces.Repositories;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Model.DTOs.Editor;
using Model.Exceptions.Editor;
using Serilog;

namespace API.Features.Editor.Hubs
{

    public class EditorHub : BaseEditorHub {

        public EditorHub(IEditorRepository editorRepository, IMapper mapper, NnaUserManager userManager, IEditorService editorService) 
            : base(editorRepository, mapper, userManager, editorService) { }


        public async Task<BaseEditorResponseDto<CreatedDmoDto>> CreateDmo(CreateDmoDto dmoDto) {
            if (!Context.ContainsUser()) {
                return null;
            }

            try {
                var result =
                    await EditorService.CreateAndLoadAsync(dmoDto, Context.GetCurrentUserId().GetValueOrDefault());
                return new BaseEditorResponseDto<CreatedDmoDto>(result);
            }
            catch (CreateDmoException ex) {
                Log.Error(ex, $"UserId: {Context.GetCurrentUserId().GetValueOrDefault()}");
                return new BaseEditorResponseDto<CreatedDmoDto>()
                    .AttachErrorDetails(new EditorErrorDetailsDto(ex.Message)
                        .CreateInternalServerError());
            }
            catch (LoadDmoException ex) {
                Log.Error(ex, $"UserId: {Context.GetCurrentUserId().GetValueOrDefault()}");
                return new BaseEditorResponseDto<CreatedDmoDto>()
                    .AttachErrorDetails(new EditorErrorDetailsDto(ex.Message)
                        .CreateInternalServerError());
            }
        }

        public async Task<ShortDmoDto> LoadDmo(string dmoId) {
            if (!Context.ContainsUser()) {
                return null;
            }
            var dmo = await EditorRepository
                .LoadShortDmoAsync(Guid.Parse(dmoId), Context.GetCurrentUserId().GetValueOrDefault());
            return dmo == null ? null : Mapper.Map<ShortDmoDto>(dmo);
        }

        public async Task<ShortDmoDto> UpdateDmoInfo(ShortDmoDto dmoDto) {
            if (!Context.ContainsUser()) {
                return null;
            }

            var updatedDmo = await EditorRepository
                .EditDmoAsync(Mapper.Map<Dmo>(dmoDto), Context.GetCurrentUserId().GetValueOrDefault());
            return updatedDmo == null ? null : Mapper.Map<ShortDmoDto>(updatedDmo);
        }




        public async Task DmoUpdate(ShortDmoWithBeatsDto dmoUpdate) {
            var dmoId = Guid.Parse(dmoUpdate.Id);
            var result = await EditorRepository.UpdateBeatsAsync(
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
