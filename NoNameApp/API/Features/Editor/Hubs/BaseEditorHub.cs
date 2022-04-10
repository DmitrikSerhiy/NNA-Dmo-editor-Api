using API.Features.Editor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Model.Interfaces;
using Serilog;
using System;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Helpers;
using API.Helpers.Extensions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Hosting;
using Model.DTOs;
using Model.DTOs.Editor.Response;
using Model.Entities;
using Model.Enums;
using Model.Interfaces.Repositories;

namespace API.Features.Editor.Hubs {
    [Authorize]
    public class BaseEditorHub : Hub {
        protected readonly IEditorService EditorService;
        protected readonly IWebHostEnvironment WebHostEnvironment;
        
        private readonly ClaimsValidator _claimsValidator;
        private readonly IUserRepository _userRepository;

        public BaseEditorHub(
            IEditorService editorService, 
            IWebHostEnvironment webHostEnvironment, 
            ClaimsValidator claimsValidator, 
            IUserRepository userRepository) {
            EditorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
            WebHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _claimsValidator = claimsValidator ?? throw new ArgumentNullException(nameof(claimsValidator));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }
        
        public override async Task OnConnectedAsync() {
            if (!(Context.User.Claims.Any(claim => claim.Type.Equals(ClaimTypes.Email)) && 
                  Context.User.Claims.Any(claim => claim.Type.Equals(ClaimTypes.NameIdentifier)) &&
                  Context.User.Claims.Any(claim => claim.Type.Equals(NnaCustomTokenClaimsDictionary.GetValue(NnaCustomTokenClaims.oid))))) {
                throw new AuthenticationException("Missing user claims");
            }

            var authData = await _claimsValidator.ValidateAndGetAuthDataAsync(Context.User.Claims.ToList());
            if (await _userRepository.HasEditorConnectionAsync(authData.UserId)) {
                throw new AuthenticationException("User already have active connection.");
            }
            
            if (WebHostEnvironment.IsLocal()) {
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

        public override async Task OnDisconnectedAsync(Exception exception) {
            if (exception != null) {
                Log.Error(exception, $"Error on websocket disconnection. User: {Context.GetCurrentUserId()}. Error: {exception.Message}");
            }

            if (WebHostEnvironment.IsLocal()) {
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
        
        protected static BaseEditorResponseDto NotValid(ValidationResult validationResult) {
            return BaseEditorResponseDto.CreateFailedValidationResponse(validationResult.Errors.Select(err =>
                new Tuple<string, string>(err.ErrorMessage, err.PropertyName)).ToList());
        }

        protected static BaseEditorResponseDto BadRequest() {
            return BaseEditorResponseDto.CreateBadRequestResponse();
        }

        protected static BaseEditorResponseDto NotAuthorized() {
            return BaseEditorResponseDto.CreateFailedAuthResponse();
        }

        protected static BaseEditorResponseDto InternalServerError(string errorMessage) {
            return BaseEditorResponseDto.CreateInternalServerErrorResponse(errorMessage);
        }

        protected static BaseEditorResponseDto NoContent() {
            return BaseEditorResponseDto.CreateNoContentResponse();
        }

        protected static EditorResponseDto<T> Ok<T>(T data) where T: BaseDto {
            return EditorResponseDto<T>.Ok(data);
        }
    }
}
