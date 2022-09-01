using NNA.Api.Features.Editor.Services;
using NNA.Api.Features.Editor.Validators;
using NNA.Api.Helpers;
using NNA.Domain.DTOs.Editor;
using NNA.Domain.Exceptions.Editor;
using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;
using Serilog;

namespace NNA.Api.Features.Editor.Hubs;

public class EditorHub : BaseEditorHub {
    public EditorHub(
        IEditorService editorService,
        IHostEnvironment webHostEnvironment,
        ClaimsValidator claimsValidator,
        IUserRepository userRepository)
        : base(editorService, webHostEnvironment, claimsValidator, userRepository) { }

    public async Task<object> LoadShortDmo(LoadShortDmoDto? dmoDto) {
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
            Log.Error(ex.InnerException!, ex.Message);
            await DisconnectUser();
            return InternalServerError(ex.Message);
        }
    }

    public async Task<object> CreateDmo(CreateDmoDto? dmoDto) {
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
            Log.Error(ex.InnerException!, ex.Message);
            await DisconnectUser();
            return InternalServerError(ex.Message);
        }
        catch (LoadShortDmoException ex) {
            Log.Error(ex.InnerException!, ex.Message);
            await DisconnectUser();
            return InternalServerError(ex.Message);
        }
    }

    public async Task UpdateShortDmo(UpdateShortDmoDto? dmoDto) {
        if (dmoDto == null) {
            await SendBackErrorResponse(BadRequest());
            return;
        }

        if (!Context.ContainsUser()) {
            await SendBackErrorResponse(NotAuthorized());
            return;
        }

        var validationResult = await new UpdateShortDmoDtoValidator().ValidateAsync(dmoDto);
        if (!validationResult.IsValid) {
            await SendBackErrorResponse(validationResult);
            return;
        }

        try {
            await EditorService.UpdateShortDmo(dmoDto, Context.GetCurrentUserId().GetValueOrDefault());
        }
        catch (UpdateShortDmoException ex) {
            Log.Error(ex.InnerException!, ex.Message);
            await DisconnectUser();
            await SendBackErrorResponse(InternalServerError(ex.Message));
        }
    }

    public async Task UpdateDmosJson(UpdateDmoBeatsAsJsonDto? update) {
        if (update == null) {
            await SendBackErrorResponse(BadRequest());
            return;
        }

        if (!Context.ContainsUser()) {
            await SendBackErrorResponse(NotAuthorized());
            return;
        }

        var validationResult = await new UpdateDmoBeatsAsJsonDtoValidator().ValidateAsync(update);
        if (!validationResult.IsValid) {
            await SendBackErrorResponse(NotValid(validationResult));
            return;
        }

        try {
            await EditorService.UpdateDmoBeatsAsJson(update, Context.GetCurrentUserId().GetValueOrDefault());
        }
        catch (UpdateDmoBeatsAsJsonException ex) {
            Log.Error(ex.InnerException!, ex.Message);
            await DisconnectUser();
            await SendBackErrorResponse(InternalServerError(ex.Message));
        }
    }

    public async Task CreateBeat(CreateBeatDto? beatDto) {
        if (beatDto == null) {
            await SendBackErrorResponse(BadRequest());
            return;
        }

        if (!Context.ContainsUser()) {
            await SendBackErrorResponse(NotAuthorized());
            return;
        }

        var validationResult = await new CreateBeatDtoValidator().ValidateAsync(beatDto);
        if (!validationResult.IsValid) {
            await SendBackErrorResponse(NotValid(validationResult));
            return;
        }

        try {
            await EditorService.CreateBeat(beatDto, Context.GetCurrentUserId().GetValueOrDefault());
        }
        catch (InsertNewBeatException ex) {
            Log.Error(ex.InnerException!, ex.Message);
            await DisconnectUser();
            await SendBackErrorResponse(InternalServerError(ex.Message));
        }
    }

    public async Task RemoveBeat(RemoveBeatDto? beatDto) {
        if (beatDto == null) {
            await SendBackErrorResponse(BadRequest());
            return;
        }

        if (!Context.ContainsUser()) {
            await SendBackErrorResponse(NotAuthorized());
            return;
        }

        var validationResult = await new RemoveBeatDtoValidator().ValidateAsync(beatDto);
        if (!validationResult.IsValid) {
            await SendBackErrorResponse(NotValid(validationResult));
            return;
        }

        try {
            await EditorService.RemoveBeat(beatDto, Context.GetCurrentUserId().GetValueOrDefault());
        }
        catch (DeleteBeatException ex) {
            Log.Error(ex.InnerException!, ex.Message);
            await DisconnectUser();
            await SendBackErrorResponse(InternalServerError(ex.Message));
        }
    }

    public async Task UpdateBeat(UpdateBeatDto? update) {
        if (update == null) {
            await SendBackErrorResponse(BadRequest());
            return;
        }

        if (!Context.ContainsUser()) {
            await SendBackErrorResponse(NotAuthorized());
            return;
        }

        var validationResult =
            await new UpdateBeatDtoValidator().ValidateAsync(update); // todo: add validators for minutes and seconds
        if (!validationResult.IsValid) {
            await SendBackErrorResponse(
                NotValid(validationResult)); // todo: check in unit tests what error was thrown exactly
            return;
        }

        try {
            await EditorService.UpdateBeat(update, Context.GetCurrentUserId().GetValueOrDefault());
        }
        catch (UpdateBeatException ex) {
            Log.Error(ex.InnerException!, ex.Message);
            await DisconnectUser(); // todo: add unit test for this line of code for every action method
            await SendBackErrorResponse(InternalServerError(ex.Message));
        }
    }
}