using API.Features.Editor.Services;
using API.Features.Editor.Validators;
using Model.DTOs.Editor;
using Model.DTOs.Editor.Response;
using Model.Exceptions.Editor;
using Model.Interfaces;
using Serilog;
using System.Threading.Tasks;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Model.Interfaces.Repositories;

namespace API.Features.Editor.Hubs {
    public class EditorHub : BaseEditorHub {

        public EditorHub(
            IEditorService editorService,
            IWebHostEnvironment webHostEnvironment,
            ClaimsValidator claimsValidator,
            IUserRepository userRepository) 
                : base(editorService, webHostEnvironment, claimsValidator, userRepository) { }
        
        public async Task<BaseEditorResponseDto> LoadShortDmo(LoadShortDmoDto dmoDto) {
            if (dmoDto == null) return BadRequest();

            if (!Context.ContainsUser()) {
                return NotAuthorized();
            }

            var validationResult = await new LoadShortDmoDtoValidator().ValidateAsync(dmoDto);
            if (!validationResult.IsValid) {
                return NotValid(validationResult);
            }

            try {
                var dmo = await EditorService.LoadShortDmo(dmoDto, Context.GetCurrentUserId().GetValueOrDefault());
                return Ok(dmo);
            }
            catch (LoadShortDmoException ex) {
                Log.Error(ex.InnerException, ex.Message);
                return InternalServerError(ex.Message);
            }
        }


        public async Task<BaseEditorResponseDto> CreateDmo(CreateDmoDto dmoDto) {
            if (dmoDto == null) return BadRequest();

            if (!Context.ContainsUser()) {
                return NotAuthorized();
            }

            var validationResult = await new CreateDmoValidator().ValidateAsync(dmoDto);
            if (!validationResult.IsValid) {
                return NotValid(validationResult);
            }

            try {
                var dmo =
                    await EditorService.CreateAndLoadDmo(dmoDto, Context.GetCurrentUserId().GetValueOrDefault());
                return Ok(dmo);
            }
            catch (CreateDmoException ex) {
                Log.Error(ex.InnerException, ex.Message);
                return InternalServerError(ex.Message);
            }
            catch (LoadShortDmoException ex) {
                Log.Error(ex.InnerException, ex.Message);
                return InternalServerError(ex.Message);
            }
        }


        public async Task<BaseEditorResponseDto> UpdateShortDmo(UpdateShortDmoDto dmoDto) {
            if (dmoDto == null) return BadRequest();

            if (!Context.ContainsUser()) {
                return NotAuthorized();
            }

            var validationResult = await new UpdateShortDmoDtoValidator().ValidateAsync(dmoDto);
            if (!validationResult.IsValid) {
                return NotValid(validationResult);
            }

            try {
                await EditorService.UpdateShortDmo(dmoDto, Context.GetCurrentUserId().GetValueOrDefault());
            }
            catch (UpdateShortDmoException ex) {
                Log.Error(ex.InnerException, ex.Message);
                return InternalServerError(ex.Message);
            }

            return NoContent();
        }

        public async Task<BaseEditorResponseDto> UpdateDmosJson(UpdateDmoBeatsAsJsonDto update) {
            if (update == null) return BadRequest();

            if (!Context.ContainsUser()) {
                return NotAuthorized();
            }

            var validationResult = await new UpdateDmoBeatsAsJsonDtoValidator().ValidateAsync(update);
            if (!validationResult.IsValid) {
                return NotValid(validationResult);
            }

            try {
                await EditorService.UpdateDmoBeatsAsJson(update, Context.GetCurrentUserId().GetValueOrDefault());
            }
            catch (UpdateDmoBeatsAsJsonException ex) {
                Log.Error(ex.InnerException, ex.Message);
                return InternalServerError(ex.Message);
            }

            return NoContent();
        }
    }
}
