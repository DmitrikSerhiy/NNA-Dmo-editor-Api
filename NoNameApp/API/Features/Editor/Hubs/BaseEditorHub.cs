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
using API.Features.Account.Services.Local;
using API.Helpers.Extensions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Hosting;
using Model.DTOs;
using Model.DTOs.Editor.Response;

namespace API.Features.Editor.Hubs {
    [Authorize]
    public class BaseEditorHub : Hub {
        protected readonly IEditorService EditorService;
        protected readonly NnaLocalUserManager LocalUserManager;
        protected readonly IWebHostEnvironment WebHostEnvironment;

        public BaseEditorHub(
            NnaLocalUserManager localUserManager,
            IEditorService editorService, 
            IWebHostEnvironment webHostEnvironment) {
            LocalUserManager = localUserManager ?? throw new ArgumentNullException(nameof(localUserManager));
            EditorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
            WebHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        }


        public override async Task OnConnectedAsync() {
            if (!(Context.User.Claims.Any(claim => claim.Type.Equals(ClaimTypes.Email)) &&
                  Context.User.Claims.Any(claim => claim.Type.Equals(ClaimTypes.NameIdentifier)))) {
                await OnDisconnectedAsync(new AuthenticationException("Missing user claims"));
                return;
            }
            
            var userId = Context.User.Claims.First(claim => claim.Type.Equals(ClaimTypes.NameIdentifier)).Value;
            var userEmail = Context.User.Claims.First(claim => claim.Type.Equals(ClaimTypes.Email)).Value;

            if (string.IsNullOrWhiteSpace(userId)) {
                await OnDisconnectedAsync(new AuthenticationException($"Invalid user id claim: '{userId}'"));
                return;
            }
            
            if (string.IsNullOrWhiteSpace(userEmail)) {
                await OnDisconnectedAsync(new AuthenticationException($"Invalid user email claim: '{userEmail}'"));
                return;
            }
            
            var user = await LocalUserManager.FindByIdAsync(userId);
            if (user is null) {
                await OnDisconnectedAsync(new AuthenticationException($"Unknown user with id claim: '{userId}'"));
                return;
            }

            if (!user.Email.Equals(userEmail, StringComparison.InvariantCultureIgnoreCase)) {
                await OnDisconnectedAsync(new AuthenticationException($"User email claim '{userEmail}' does not correspond to user id claim: '{userId}'"));
                return;
            } 
            
            Context.AuthenticateUser(user);
            if (WebHostEnvironment.IsLocal()) {
                Console.WriteLine($"{Context.GetCurrentUserEmail()} just connected to the editor");
            }

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception) {
            if (exception != null) {
                Log.Error(exception, $"Error on websocket disconnection. User: {Context.GetCurrentUserId()}. Error: {exception.Message}");
            }

            if (WebHostEnvironment.IsLocal()) {
                Console.WriteLine($"{Context.GetCurrentUserEmail()} disconnected from the editor");
            }

            Context.LogoutUser();
            return base.OnDisconnectedAsync(exception);
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
