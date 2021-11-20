using API.Features.Editor.Hubs;
using Microsoft.AspNetCore.Identity;
using Model.Entities;
using Model.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using API.Features.Account.Services;
using API.Features.Account.Services.Local;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;

namespace Tests.EditorHubTests
{
    public class BaseEditorTests {
        protected EditorHub Subject { get; set; }
        protected Mock<IEditorService> EditorServiceMock { get; private set; }
        protected Mock<NnaLocalUserManager> UserManagerMock { get; private set; }
        protected Mock<IWebHostEnvironment> EnvironmentMock { get; private set; }

        protected string UserName { get; } = "UserName";
        protected string UserEmail { get; } = "User@gmail.com";
        protected Guid UserId { get; } = Guid.NewGuid();

        protected void SetupConstructorMocks() {
            EditorServiceMock = new Mock<IEditorService>();

            UserManagerMock = new Mock<NnaLocalUserManager>(Mock.Of<IUserStore<NnaUser>>(), null, null, null, null, null, null, null, null);
            UserManagerMock.Object.UserValidators.Add(new UserValidator<NnaUser>());
            UserManagerMock.Object.PasswordValidators.Add(new PasswordValidator<NnaUser>());
            UserManagerMock.Setup(x => x.DeleteAsync(It.IsAny<NnaUser>())).ReturnsAsync(IdentityResult.Success);
            UserManagerMock.Setup(x => x.CreateAsync(It.IsAny<NnaUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            UserManagerMock.Setup(x => x.UpdateAsync(It.IsAny<NnaUser>())).ReturnsAsync(IdentityResult.Success);
            EnvironmentMock = new Mock<IWebHostEnvironment>();
        }

        protected void SetupHubContext() {
            if (Subject == null) {
                return;
            }

            var hubContext = new Mock<HubCallerContext>();

            var userMock = new Mock<NnaUser>();
            userMock.SetupProperty(s => s.UserName, UserName);
            userMock.SetupProperty(s => s.Email, UserEmail);
            userMock.SetupProperty(s => s.Id, UserId);

            var authProvider = new AuthenticatedIdentityProvider();
            // todo: fix this test
            // authProvider.SetAuthenticatedUser(userMock.Object);
            var items = new Dictionary<object, object> { { "user", authProvider } };
            hubContext.Setup(hm => hm.Items).Returns(items);
            Subject.Context = hubContext.Object;
        }
    }
}
