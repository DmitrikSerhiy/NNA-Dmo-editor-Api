using API.Features.Account.Services;
using API.Features.Editor.Services;
using API.Features.Editor.Validators;
using AutoMapper;
using Model.DTOs.Editor;
using Model.DTOs.Editor.Response;
using Model.Exceptions.Editor;
using Model.Interfaces;
using Serilog;
using System;
using System.Threading.Tasks;

namespace API.Features.Editor.Hubs {
    public class EditorHub : BaseEditorHub {

        public EditorHub(IMapper mapper, NnaUserManager userManager, IEditorService editorService) 
            : base(mapper, userManager, editorService) { }


        public async Task<BaseEditorResponseDto> LoadShortDmo(LoadShortDmoDto dmoDto) {
            if (dmoDto == null) throw new ArgumentNullException(nameof(dmoDto));

            if (!Context.ContainsUser()) {
                return NotAuthorized();
            }

            var validationResult = await new LoadShortDmoDtoValidator().ValidateAsync(dmoDto);
            if (!validationResult.IsValid) {
                return NotValid(validationResult);
            }

            try {
                var dmo = await EditorService.LoadShortDmo(dmoDto, Context.GetCurrentUserId().GetValueOrDefault());
                return new EditorResponseDto<LoadedShortDmoDto>(dmo);
            }
            catch (LoadShortDmoException ex) {
                Log.Error(ex.InnerException, ex.Message);
                return InternalServerError(ex.Message);
            }
        }


        public async Task<BaseEditorResponseDto> CreateDmo(CreateDmoDto dmoDto) {
            if (dmoDto == null) throw new ArgumentNullException(nameof(dmoDto));

            if (!Context.ContainsUser()) {
                return NotAuthorized();
            }

            var validationResult = await new CreateDmoValidator().ValidateAsync(dmoDto);
            if (!validationResult.IsValid) {
                return NotValid(validationResult);
            }

            try {
                var dmo =
                    await EditorService.CreateAndLoadAsync(dmoDto, Context.GetCurrentUserId().GetValueOrDefault());
                return new EditorResponseDto<CreatedDmoDto>(dmo);
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
            if (dmoDto == null) throw new ArgumentNullException(nameof(dmoDto));

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
    }
}
