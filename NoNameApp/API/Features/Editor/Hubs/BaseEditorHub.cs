using API.Features.Account.Services;
using API.Features.Editor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Model.Interfaces;
using Serilog;
using System;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using FluentValidation.Results;
using Model.DTOs;
using Model.DTOs.Editor.Response;

namespace API.Features.Editor.Hubs {
    [Authorize]
    public class BaseEditorHub : Hub {
        protected readonly IEditorService EditorService;
        protected readonly NnaUserManager UserManager;

        public BaseEditorHub(
            NnaUserManager userManager,
            IEditorService editorService) {
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            EditorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
        }


        public override async Task OnConnectedAsync() {
            var userName = Context.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
            if (userName == null) {
                await OnDisconnectedAsync(new AuthenticationException("User is not authenticated [websocket]"));
                return;
            }

            var user = await UserManager.FindByNameAsync(userName.Value);
            if (user == null) {
                await OnDisconnectedAsync(new AuthenticationException("User not found [websocket]"));
                return;
            }
            Context.AuthenticateUser(user);
            Console.WriteLine($"{Context.GetCurrentUserEmail()} just connected to the editor");
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception) {
            if (exception != null) {
                Log.Error(exception, $"Error on websocket disconnection. User: {Context.GetCurrentUserId()}. Error: {exception.Message}");
            }

            Console.WriteLine($"{Context.GetCurrentUserEmail()} disconnected from the editor");
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
