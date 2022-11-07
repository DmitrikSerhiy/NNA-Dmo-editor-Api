﻿using NNA.Api.Features.Editor.Services;
using NNA.Api.Features.Editor.Validators;
using NNA.Api.Helpers;
using NNA.Domain.DTOs.CharactersInBeats;
using NNA.Domain.DTOs.Editor;
using NNA.Domain.Exceptions.Editor;
using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;
using Serilog;

namespace NNA.Api.Features.Editor.Hubs;

public sealed class EditorHub : BaseEditorHub {
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
            Log.Error(ex.InnerException ?? new Exception("No inner exception"), ex.Message);
            await DisconnectUser();
            return InternalServerError(ex.Message);
        }
    }
    
    [Obsolete]
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
            Log.Error(ex.InnerException ?? new Exception("No inner exception"), ex.Message);
            await DisconnectUser();
            return InternalServerError(ex.Message);
        }
        catch (LoadShortDmoException ex) {
            Log.Error(ex.InnerException ?? new Exception("No inner exception"), ex.Message);
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
            Log.Error(ex.InnerException ?? new Exception("No inner exception"), ex.Message);
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
            Log.Error(ex.InnerException ?? new Exception("No inner exception"), ex.Message);
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
            Log.Error(ex.InnerException ?? new Exception("No inner exception"), ex.Message);
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
            Log.Error(ex.InnerException ?? new Exception("No inner exception"), ex.Message);
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
            await new UpdateBeatDtoValidator().ValidateAsync(update);
        if (!validationResult.IsValid) {
            await SendBackErrorResponse(
                NotValid(validationResult)); // todo: check in unit tests what error was thrown exactly
            return;
        }

        try {
            await EditorService.UpdateBeat(update, Context.GetCurrentUserId().GetValueOrDefault());
        }
        catch (UpdateBeatException ex) {
            Log.Error(ex.InnerException ?? new Exception("No inner exception"), ex.Message);
            await DisconnectUser();
            await SendBackErrorResponse(InternalServerError(ex.Message));
        }
    }
    
    public async Task SwapBeats(SwapBeatsDto? update) {
        if (update == null) {
            await SendBackErrorResponse(BadRequest());
            return;
        }
        
        if (!Context.ContainsUser()) {
            await SendBackErrorResponse(NotAuthorized());
            return;
        }
        
        var validationResult = await new SwapBeatsDtoValidator().ValidateAsync(update);
        if (!validationResult.IsValid) {
            await SendBackErrorResponse(NotValid(validationResult));
            return;
        }

        try {
            await EditorService.SwapBeats(update, Context.GetCurrentUserId().GetValueOrDefault());
        }
        catch (SwapBeatsException ex) {
            Log.Error(ex.InnerException ?? new Exception("No inner exception"), ex.Message);
            await DisconnectUser();
            await SendBackErrorResponse(InternalServerError(ex.Message));
        }
    }
    
    public async Task AttachCharacterToBeat(AttachCharacterToBeatDto? characterToBeatDto) {
        if (characterToBeatDto == null) {
            await SendBackErrorResponse(BadRequest());
            return;
        }
        
        if (!Context.ContainsUser()) {
            await SendBackErrorResponse(NotAuthorized());
            return;
        }
        
        var validationResult = await new AttachCharacterToBeatDtoValidator().ValidateAsync(characterToBeatDto);
        if (!validationResult.IsValid) {
            await SendBackErrorResponse(NotValid(validationResult));
            return;
        }

        try {
            await EditorService.AttachCharacterToBeat(characterToBeatDto, Context.GetCurrentUserId().GetValueOrDefault());
        }
        catch (AttachCharacterToBeatException ex) {
            Log.Error(ex.InnerException ?? new Exception("No inner exception"), ex.Message);
            await DisconnectUser();
            await SendBackErrorResponse(InternalServerError(ex.Message));
        }
    }
    
    public async Task DetachCharacterFromBeat(DetachCharacterToBeatDto? characterToDetachDto) {
        if (characterToDetachDto == null) {
            await SendBackErrorResponse(BadRequest());
            return;
        }
        
        if (!Context.ContainsUser()) {
            await SendBackErrorResponse(NotAuthorized());
            return;
        }
        
        var validationResult = await new DetachCharacterToBeatDtoValidator().ValidateAsync(characterToDetachDto);
        if (!validationResult.IsValid) {
            await SendBackErrorResponse(NotValid(validationResult));
            return;
        }

        try {
            await EditorService.DetachCharacterFromBeat(characterToDetachDto, Context.GetCurrentUserId().GetValueOrDefault());
        }
        catch (RemoveCharacterFromBeatException ex) {
            Log.Error(ex.InnerException ?? new Exception("No inner exception"), ex.Message);
            await DisconnectUser();
            await SendBackErrorResponse(InternalServerError(ex.Message));
        }
    }
    
    
    [Obsolete("Moved to Http end-point")]
    public async Task SanitizeTempIds(SanitizeTempIdsDto? update) {
        if (update == null) {
            await SendBackErrorResponse(BadRequest());
            return;
        }
        
        if (!Context.ContainsUser()) {
            await SendBackErrorResponse(NotAuthorized());
            return;
        }
        
        var validationResult = await new SanitizeTempIdsDtoValidator().ValidateAsync(update);
        if (!validationResult.IsValid) {
            await SendBackErrorResponse(NotValid(validationResult));
            return;
        }
        
        try {
            await EditorService.SanitizeTempIds(update, Context.GetCurrentUserId().GetValueOrDefault());
        }
        catch (SanitizeTempIdsException ex) {
            Log.Error(ex.InnerException ?? new Exception("No inner exception"), ex.Message);
            await DisconnectUser();
            await SendBackErrorResponse(InternalServerError(ex.Message));
        }
    }
}