using Model.Entities;
using Model.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Model.Interfaces.Repositories;
using NNA.Api.Features.Account.Services;
using NNA.Api.Features.Editor.Hubs;
using NNA.Api.Helpers;

namespace Tests.EditorHubTests; 
public class BaseEditorTests {
    protected EditorHub Subject { get; set; }
    protected Mock<IEditorService> EditorServiceMock { get; private set; }
    protected Mock<IHostEnvironment> EnvironmentMock { get; private set; }
    protected Mock<ClaimsValidator> ClaimsValidatorMock { get; private set; }
    protected Mock<IUserRepository> UserRepositoryMock { get; private set; }

    protected Mock<IHubCallerClients<IEditorClient>> EditorClientsMock { get; private set; }


    protected string UserName { get; } = "UserName";
    protected string UserEmail { get; } = "User@gmail.com";
    protected Guid UserId { get; } = Guid.NewGuid();
        

    protected void SetupConstructorMocks() {
        EditorServiceMock = new Mock<IEditorService>();
        EnvironmentMock = new Mock<IHostEnvironment>();
        ClaimsValidatorMock = new Mock<ClaimsValidator>();
        UserRepositoryMock = new Mock<IUserRepository>();
    }

    protected void SetupHubContext() {
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
        var items = new Dictionary<object, object> { { "user", authProvider } };
        hubContext.Setup(hm => hm.Items).Returns(items);
        Subject.Context = hubContext.Object;
            
        EditorClientsMock = new Mock<IHubCallerClients<IEditorClient>>();
        EditorClientsMock.Setup(client => client.Caller).Returns(new Mock<EditorClient>().Object);
        Subject.Clients = EditorClientsMock.Object;
    }
        
    public class EditorClient : IEditorClient {
        public virtual Task OnServerError(object notificationBody) {
            return Task.CompletedTask;
        }
    }
}