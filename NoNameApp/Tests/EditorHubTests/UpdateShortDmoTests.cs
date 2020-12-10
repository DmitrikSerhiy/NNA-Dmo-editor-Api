using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Features.Editor.Hubs;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Model.DTOs.Editor;
using Model.DTOs.Editor.Response;
using Model.Exceptions.Editor;
using Moq;
using Xunit;

namespace Tests.EditorHubTests {
    public class UpdateShortDmoTests : BaseEditorTests {

        private UpdateShortDmoDto DmoDto { get; set; }

        private void SetMockAndVariables() {
            SetupConstructorMocks();
            DmoDto = new UpdateShortDmoDto {
                Id = Guid.NewGuid().ToString(),
                MovieTitle = "movie title",
                Name = "dmo name",
                ShortComment = "some comment"
            };
        }

        [Fact]
        public async Task ShouldReturnBadRequestIfEntryDtoIsEmptyTest() {
            //Arrange
            SetMockAndVariables();
            Subject = new EditorHub(UserManagerMock.Object, EditorServiceMock.Object);
            SetupHubContext();

            //Act
            Func<Task<BaseEditorResponseDto>> act = async () => await Subject.UpdateShortDmo(null);
            var response = await act.Invoke();

            //Assert
            response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateBadRequestResponse());
        }

        [Fact]
        public async Task ShouldReturnNotAuthorizedIfNoUserInContextTest() {
            //Arrange
            SetMockAndVariables();
            Subject = new EditorHub(UserManagerMock.Object, EditorServiceMock.Object);
            var hubContext = new Mock<HubCallerContext>();
            hubContext.Setup(hm => hm.Items).Returns(new Dictionary<object, object>());
            Subject.Context = hubContext.Object;

            //Act
            Func<Task<BaseEditorResponseDto>> act = async () => await Subject.UpdateShortDmo(DmoDto);
            var response = await act.Invoke();

            //Assert
            response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateFailedAuthResponse());
        }

        [Fact]
        public async Task ShouldReturnNotValidResponseIfDtoIsNotValidTest() {
            //Arrange
            SetMockAndVariables();
            DmoDto.Id = null;
            Subject = new EditorHub(UserManagerMock.Object, EditorServiceMock.Object);
            SetupHubContext();

            //Act
            Func<Task<BaseEditorResponseDto>> act = async () => await Subject.UpdateShortDmo(DmoDto);
            var response = await act.Invoke();

            //Assert
            response.HttpCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        }

        [Fact]
        public async Task ShouldReturnNoContentResponseTest() {
            //Arrange
            SetMockAndVariables();

            EditorServiceMock.Setup(esm => esm.UpdateShortDmo(DmoDto, UserId)).Verifiable();
            Subject = new EditorHub(UserManagerMock.Object, EditorServiceMock.Object);
            SetupHubContext();

            //Act
            Func<Task<BaseEditorResponseDto>> act = async () => await Subject.UpdateShortDmo(DmoDto);
            var response = await act.Invoke();

            //Assert
            response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateNoContentResponse());
            EditorServiceMock.Verify(esm => esm.UpdateShortDmo(DmoDto, UserId), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnInternalServerErrorResponseIfRepositoryThrowsTest() {
            //Arrange
            SetMockAndVariables();
            var exceptionMessage = "some message";

            EditorServiceMock.Setup(esm => esm.UpdateShortDmo(DmoDto, UserId))
                .ThrowsAsync(new UpdateShortDmoException(exceptionMessage, new Exception("exception from repository")));
            Subject = new EditorHub(UserManagerMock.Object, EditorServiceMock.Object);
            SetupHubContext();

            //Act
            Func<Task<BaseEditorResponseDto>> act = async () => await Subject.UpdateShortDmo(DmoDto);
            var response = await act.Invoke();

            //Assert
            response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateInternalServerErrorResponse($"{UpdateShortDmoException.CustomMessage} {exceptionMessage}"));
        }

    }
}
