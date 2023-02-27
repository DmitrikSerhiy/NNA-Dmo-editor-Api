using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Moq;
using NNA.Api.Features.Account.Services;
using NNA.Api.Features.Editor.Hubs;
using NNA.Api.Helpers;
using NNA.Domain.Entities;
using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Tests.EditorHubTests;

public class BaseEditorTests {
    protected EditorHub? Subject { get; set; } = null!;
    protected Mock<IEditorService> EditorServiceMock { get; private set; } = null!;
    protected Mock<IHostEnvironment> EnvironmentMock { get; private set; } = null!;
    protected Mock<ClaimsValidator> ClaimsValidatorMock { get; private set; } = null!;
    protected Mock<IUserRepository> UserRepositoryMock { get; private set; } = null!;

    protected Mock<IHubCallerClients<IEditorClient>> EditorClientsMock { get; private set; } = null!;


    protected string UserName { get; } = "UserName";
    protected string UserEmail { get; } = "User@gmail.com";
    protected string ConnectionId { get; } = "CurrentConnectionId";
    protected Guid UserId { get; } = Guid.NewGuid();

    protected EditorConnection EditorConnection { get; set; } = null!;

    protected void SetupConstructorMocks() {
        EditorServiceMock = new Mock<IEditorService>();
        EnvironmentMock = new Mock<IHostEnvironment>();
        ClaimsValidatorMock = new Mock<ClaimsValidator>();
        UserRepositoryMock = new Mock<IUserRepository>();
        EditorConnection = new EditorConnection { UserId = UserId, ConnectionId = ConnectionId };
    }

    protected void SetupHubContext() {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (Subject == null) {
            return;
        }

        var hubContext = new Mock<HubCallerContext>();

        var userMock = new Mock<UsersTokens>();
        userMock.SetupProperty(s => s.Email, UserEmail);
        userMock.SetupProperty(s => s.AccessTokenId, "AccessTokenValue");
        userMock.SetupProperty(s => s.RefreshTokenId, "RefreshTokenValue");
        userMock.SetupProperty(s => s.UserId, UserId);


        var authProvider = new AuthenticatedIdentityProvider();
        authProvider.SetAuthenticatedUser(userMock.Object);
        var items = new Dictionary<object, object?> { { "user", authProvider } };
        hubContext.Setup(hm => hm.Items).Returns(items);
        hubContext.Setup(hm => hm.ConnectionId).Returns(ConnectionId);
        // hubContext.Setup(hm => hm.LogoutUser)
        Subject.Context = hubContext.Object;

        EditorClientsMock = new Mock<IHubCallerClients<IEditorClient>>();
        EditorClientsMock.Setup(client => client.Caller).Returns(new Mock<EditorClient>().Object);
        Subject.Clients = EditorClientsMock.Object;
    }

    public  class EditorClient : IEditorClient {
        public virtual Task OnServerError(object notificationBody) {
            return Task.CompletedTask;
        }
    }
}