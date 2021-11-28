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

namespace Tests.EditorHubTests
{
    public class UpdateDmosBeatsJsonTests : BaseEditorTests {

        // ReSharper disable once InconsistentNaming
        private UpdateDmoBeatsAsJsonDto update { get; set; }

        private void SetMockAndVariables() {
            SetupConstructorMocks();
            update = new UpdateDmoBeatsAsJsonDto { DmoId = Guid.NewGuid().ToString(), Json = "{}"};
        }


        [Fact]
        public async Task ShouldReturnBadRequestIfEntryDtoIsEmptyTest() {
            //Arrange
            SetMockAndVariables();
            Subject = new EditorHub(
                EditorServiceMock.Object, 
                EnvironmentMock.Object,
                ClaimsValidatorMock.Object,
                UserRepositoryMock.Object);
            SetupHubContext();

            //Act
            Func<Task<BaseEditorResponseDto>> act = async () => await Subject.UpdateDmosJson(null);
            var response = await act.Invoke();

            //Assert
            response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateBadRequestResponse());
        }


        [Fact]
        public async Task ShouldReturnNotAuthorizedIfNoUserInContextTest() {
            //Arrange
            SetMockAndVariables();
            Subject = new EditorHub(
                EditorServiceMock.Object, 
                EnvironmentMock.Object,
                ClaimsValidatorMock.Object,
                UserRepositoryMock.Object);
            var hubContext = new Mock<HubCallerContext>();
            hubContext.Setup(hm => hm.Items).Returns(new Dictionary<object, object>());
            Subject.Context = hubContext.Object;

            //Act
            Func<Task<BaseEditorResponseDto>> act = async () => await Subject.UpdateDmosJson(update);
            var response = await act.Invoke();

            //Assert
            response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateFailedAuthResponse());
        }

        [Fact]
        public async Task ShouldReturnNotValidResponseIfDtoIsNotValidTest() {
            //Arrange
            SetMockAndVariables();
            update.DmoId = null;
            Subject = new EditorHub(
                EditorServiceMock.Object, 
                EnvironmentMock.Object,
                ClaimsValidatorMock.Object,
                UserRepositoryMock.Object);
            SetupHubContext();

            //Act
            Func<Task<BaseEditorResponseDto>> act = async () => await Subject.UpdateDmosJson(update);
            var response = await act.Invoke();

            //Assert
            response.HttpCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        }

        [Fact]
        public async Task ShouldReturnNoContentResponseTest() {
            //Arrange
            SetMockAndVariables();

            EditorServiceMock.Setup(esm => esm.UpdateDmoBeatsAsJson(update, UserId)).Verifiable();
            Subject = new EditorHub(
                EditorServiceMock.Object, 
                EnvironmentMock.Object,
                ClaimsValidatorMock.Object,
                UserRepositoryMock.Object);
            SetupHubContext();

            //Act
            Func<Task<BaseEditorResponseDto>> act = async () => await Subject.UpdateDmosJson(update);
            var response = await act.Invoke();

            //Assert
            response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateNoContentResponse());
            EditorServiceMock.Verify(esm => esm.UpdateDmoBeatsAsJson(update, UserId), Times.Once);
        }


        [Fact]
        public async Task ShouldReturnInternalServerErrorResponseIfRepositoryThrowsTest() {
            //Arrange
            SetMockAndVariables();
            var exceptionMessage = "some message";

            EditorServiceMock.Setup(esm => esm.UpdateDmoBeatsAsJson(update, UserId))
                .ThrowsAsync(new UpdateDmoBeatsAsJsonException(exceptionMessage, new Exception("exception from repository")));
            Subject = new EditorHub(
                EditorServiceMock.Object, 
                EnvironmentMock.Object,
                ClaimsValidatorMock.Object,
                UserRepositoryMock.Object);
            SetupHubContext();

            //Act
            Func<Task<BaseEditorResponseDto>> act = async () => await Subject.UpdateDmosJson(update);
            var response = await act.Invoke();

            //Assert
            response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateInternalServerErrorResponse($"{UpdateDmoBeatsAsJsonException.CustomMessage} {exceptionMessage}"));
        }

    }
}
