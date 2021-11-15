using API.Features.Editor.Hubs;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Model.DTOs.Editor;
using Model.DTOs.Editor.Response;
using Model.Exceptions.Editor;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Tests.EditorHubTests {
    public class LoadShortTests : BaseEditorTests {

        private LoadShortDmoDto DmoDto { get; set; }

        private void SetMockAndVariables() {
            SetupConstructorMocks();
            DmoDto = new LoadShortDmoDto {Id = Guid.NewGuid().ToString()};
        }


        [Fact]
        public async Task ShouldReturnBadRequestIfEntryDtoIsEmptyTest() {
            //Arrange
            SetMockAndVariables();
            Subject = new EditorHub(UserManagerMock.Object, EditorServiceMock.Object, EnvironmentMock.Object);
            SetupHubContext();

            //Act
            Func<Task<BaseEditorResponseDto>> act = async () => await Subject.LoadShortDmo(null);
            var response = await act.Invoke();

            //Assert
            response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateBadRequestResponse());
        }

        [Fact]
        public async Task ShouldReturnNotAuthorizedIfNoUserInContextTest() {
            //Arrange
            SetMockAndVariables();
            Subject = new EditorHub(UserManagerMock.Object, EditorServiceMock.Object, EnvironmentMock.Object);
            var hubContext = new Mock<HubCallerContext>();
            hubContext.Setup(hm => hm.Items).Returns(new Dictionary<object, object>());
            Subject.Context = hubContext.Object;

            //Act
            Func<Task<BaseEditorResponseDto>> act = async () => await Subject.LoadShortDmo(DmoDto);
            var response = await act.Invoke();

            //Assert
            response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateFailedAuthResponse());
        }

        [Fact]
        public async Task ShouldReturnNotValidResponseIfDtoIsNotValidTest() {
            //Arrange
            SetMockAndVariables();
            DmoDto.Id = null;
            Subject = new EditorHub(UserManagerMock.Object, EditorServiceMock.Object, EnvironmentMock.Object);
            SetupHubContext();

            //Act
            Func<Task<BaseEditorResponseDto>> act = async () => await Subject.LoadShortDmo(DmoDto);
            var response = await act.Invoke();

            //Assert
            response.HttpCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        }

        [Fact]
        public async Task ShouldReturnShortDmoTest() {
            //Arrange
            SetMockAndVariables();
            var loadedDmo = new LoadedShortDmoDto();

            EditorServiceMock.Setup(esm => esm.LoadShortDmo(DmoDto, UserId)).ReturnsAsync(loadedDmo);
            Subject = new EditorHub(UserManagerMock.Object, EditorServiceMock.Object, EnvironmentMock.Object);
            SetupHubContext();

            //Act
            Func<Task<BaseEditorResponseDto>> act = async () => await Subject.LoadShortDmo(DmoDto);
            var response = await act.Invoke();

            //Assert
            response.Should().BeEquivalentTo(EditorResponseDto<LoadedShortDmoDto>.Ok(loadedDmo));
        }

        [Fact]
        public async Task ShouldReturnInternalServerErrorResponseIfRepositoryThrowsTest() {
            //Arrange
            SetMockAndVariables();
            var exceptionMessage = "some message";

            EditorServiceMock.Setup(esm => esm.LoadShortDmo(DmoDto, UserId))
                .ThrowsAsync(new LoadShortDmoException(exceptionMessage, new Exception("exception from repository")));
            Subject = new EditorHub(UserManagerMock.Object, EditorServiceMock.Object, EnvironmentMock.Object);
            SetupHubContext();

            //Act
            Func<Task<BaseEditorResponseDto>> act = async () => await Subject.LoadShortDmo(DmoDto);
            var response = await act.Invoke();

            //Assert
            response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateInternalServerErrorResponse($"{LoadShortDmoException.CustomMessage} {exceptionMessage}"));
        }
    }
}
