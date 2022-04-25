using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Features.Editor.Hubs;
using FluentAssertions;
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
            Subject = new EditorHub(
                EditorServiceMock.Object, 
                EnvironmentMock.Object,
                ClaimsValidatorMock.Object,
                UserRepositoryMock.Object);
            SetupHubContext();

            //Act
            Func<Task<object>> act = async () => await Subject.UpdateShortDmo(null);
            var response = await act.Invoke();

            //Assert
            response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateBadRequestResponse(), 
                config => config
                    .Excluding(exclude => exclude.errors)
                    .Excluding(exclude => exclude.warnings)
                    .Excluding(exclude => exclude.message)
            );
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
            Func<Task<object>> act = async () => await Subject.UpdateShortDmo(DmoDto);
            var response = await act.Invoke();

            //Assert
            response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateFailedAuthResponse(),
                config => config
                    .Excluding(exclude => exclude.warnings));        }

        [Fact]
        public async Task ShouldReturnNotValidResponseIfDtoIsNotValidTest() {
            //Arrange
            SetMockAndVariables();
            DmoDto.Id = null;
            Subject = new EditorHub(
                EditorServiceMock.Object, 
                EnvironmentMock.Object,
                ClaimsValidatorMock.Object,
                UserRepositoryMock.Object);
            SetupHubContext();

            //Act
            Func<Task<object>> act = async () => await Subject.UpdateShortDmo(DmoDto);
            var response = await act.Invoke();

            //Assert
            response.Should().BeEquivalentTo(
                BaseEditorResponseDto.CreateFailedValidationResponse(new List<Tuple<string, string>>()),
                config => config
                    .Including(include => include.httpCode)
                    .Including(include => include.header)
                    .Including(include => include.message)
                    .Including(include => include.isSuccessful));
        }

        [Fact]
        public async Task ShouldReturnNoContentResponseTest() {
            //Arrange
            SetMockAndVariables();

            EditorServiceMock.Setup(esm => esm.UpdateShortDmo(DmoDto, UserId)).Verifiable();
            Subject = new EditorHub(
                EditorServiceMock.Object, 
                EnvironmentMock.Object,
                ClaimsValidatorMock.Object,
                UserRepositoryMock.Object);
            SetupHubContext();

            //Act
            Func<Task<object>> act = async () => await Subject.UpdateShortDmo(DmoDto);
            var response = await act.Invoke();

            //Assert
            response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateNoContentResponse(),
                config => config
                    .Excluding(exclude => exclude.errors)
                    .Excluding(exclude => exclude.warnings)
                    .Excluding(exclude => exclude.message));
            EditorServiceMock.Verify(esm => esm.UpdateShortDmo(DmoDto, UserId), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnInternalServerErrorResponseIfRepositoryThrowsTest() {
            //Arrange
            SetMockAndVariables();
            var exceptionMessage = "some message";

            EditorServiceMock.Setup(esm => esm.UpdateShortDmo(DmoDto, UserId))
                .ThrowsAsync(new UpdateShortDmoException(exceptionMessage, new Exception("exception from repository")));
            Subject = new EditorHub(
                EditorServiceMock.Object, 
                EnvironmentMock.Object,
                ClaimsValidatorMock.Object,
                UserRepositoryMock.Object);
            SetupHubContext();

            //Act
            Func<Task<object>> act = async () => await Subject.UpdateShortDmo(DmoDto);
            var response = await act.Invoke();

            //Assert
            response.Should().BeEquivalentTo(
                BaseEditorResponseDto.CreateInternalServerErrorResponse($"{UpdateShortDmoException.CustomMessage} {exceptionMessage}"),
                config => config
                    .Excluding(exclude => exclude.warnings));
        }

    }
}
