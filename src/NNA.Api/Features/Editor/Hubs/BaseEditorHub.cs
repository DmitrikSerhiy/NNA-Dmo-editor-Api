using System.Security.Authentication;
using System.Security.Claims;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NNA.Api.Extensions;
using NNA.Api.Features.Editor.Services;
using NNA.Api.Helpers;
using NNA.Domain.DTOs;
using NNA.Domain.DTOs.Editor.Response;
using NNA.Domain.Entities;
using NNA.Domain.Enums;
using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;
using Serilog;

namespace NNA.Api.Features.Editor.Hubs;

[Authorize]
public class BaseEditorHub : Hub<IEditorClient> {
    protected readonly IEditorService EditorService;
    protected readonly IHostEnvironment Environment;

    private readonly ClaimsValidator _claimsValidator;
    private readonly IUserRepository _userRepository;

    public BaseEditorHub(
        IEditorService editorService,
        IHostEnvironment hostEnvironment,
        ClaimsValidator claimsValidator,
        IUserRepository userRepository) {
        EditorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
        Environment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        _claimsValidator = claimsValidator ?? throw new ArgumentNullException(nameof(claimsValidator));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public override async Task OnConnectedAsync() {
        if (!(Context.User!.Claims.Any(claim => claim.Type.Equals(ClaimTypes.Email)) &&
              Context.User.Claims.Any(claim => claim.Type.Equals(ClaimTypes.NameIdentifier)) &&
              Context.User.Claims.Any(claim =>
                  claim.Type.Equals(NnaCustomTokenClaimsDictionary.GetValue(NnaCustomTokenClaims.oid))))) {
            throw new AuthenticationException("Missing user claims");
        }

        var authData = await _claimsValidator.ValidateAndGetAuthDataAsync(Context.User.Claims.ToList());
        if (await _userRepository.HasEditorConnectionAsync(authData.UserId)) {
            throw new AuthenticationException("User already have active connection.");
        }

        if (Environment.IsLocal()) {
            Console.WriteLine($"{Context.GetCurrentUserEmail()} just connected to the editor");
        }

        await _userRepository.AddEditorConnectionAsync(new EditorConnection {
            UserId = authData.UserId,
            ConnectionId = Context.ConnectionId
        });
        await _userRepository.SyncContextImmediatelyAsync();
        Context.AuthenticateUser(authData);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception) {
        if (exception != null) {
            Log.Error(exception,
                $"Error on websocket disconnection. User: {Context.GetCurrentUserId()}. Error: {exception.Message}");
        }

        if (Environment.IsLocal()) {
            Console.WriteLine($"{Context.GetCurrentUserEmail()} disconnected from the editor");
        }

        await DisconnectUser();
        await base.OnDisconnectedAsync(exception);
    }

    protected async Task DisconnectUser() {
        _userRepository.RemoveEditorConnection(new EditorConnection {
            UserId = Context.GetCurrentUserId().GetValueOrDefault(),
            ConnectionId = Context.ConnectionId
        });
        await _userRepository.SyncContextImmediatelyAsync();
        Context.LogoutUser();
    }

    public virtual async Task SendBackErrorResponse(object response) {
        await Clients.Caller.OnServerError(response);
    }

    protected static object NotValid(ValidationResult validationResult) {
        var result = BaseEditorResponseDto.CreateFailedValidationResponse(validationResult.Errors.Select(err =>
            new Tuple<string, string>(err.ErrorMessage, err.PropertyName)).ToList());

        return new {
            result.httpCode,
            result.header,
            result.message,
            result.isSuccessful,
            warnings = result.warnings.Select(warning =>
                new { fieldName = warning.FieldName, validationMessage = warning.ValidationMessage }).ToArray()
        };
    }

    protected static object BadRequest() {
        var result = BaseEditorResponseDto.CreateBadRequestResponse();
        return new {
            result.httpCode,
            result.header,
            result.isSuccessful
        };
    }

    protected static object NotAuthorized() {
        var result = BaseEditorResponseDto.CreateFailedAuthResponse();
        return new {
            result.header,
            result.message,
            result.isSuccessful,
            result.httpCode,
            errors = new object[] { new { result.errors.First().errorMessage } },
        };
    }

    protected static object InternalServerError(string errorMessage) {
        var result = BaseEditorResponseDto.CreateInternalServerErrorResponse(errorMessage);
        return new {
            errors = new object[] { new { errorMessage } },
            result.header,
            result.message,
            result.httpCode,
            result.isSuccessful
        };
    }

    protected static object NoContent() {
        var result = BaseEditorResponseDto.CreateNoContentResponse();
        return new {
            result.header,
            result.httpCode,
            result.isSuccessful
        };
    }

    protected static object Ok<T>(T data) where T : BaseDto {
        var result = EditorResponseDto<T>.Ok(data);
        return new {
            result.data,
            result.header,
            result.httpCode,
            result.isSuccessful
        };
    }
}
