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
    public class CreateDmoTests : BaseEditorTests {

        private CreateDmoDto DmoDto { get; set; }

        private void SetMockAndVariables()
        {
            SetupConstructorMocks();
            DmoDto = new CreateDmoDto {
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
            Func<Task<BaseEditorResponseDto>> act = async () => await Subject.CreateDmo(null);
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
            Func<Task<BaseEditorResponseDto>> act = async () => await Subject.CreateDmo(DmoDto);
            var response = await act.Invoke();

            //Assert
            response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateFailedAuthResponse());
        }

        [Fact]
        public async Task ShouldReturnNotValidResponseIfDtoIsNotValidTest() {
            //Arrange
            SetMockAndVariables();
            DmoDto.Name = null;
            Subject = new EditorHub(
                EditorServiceMock.Object, 
                EnvironmentMock.Object, 
                ClaimsValidatorMock.Object,
                UserRepositoryMock.Object);
            SetupHubContext();

            //Act
            Func<Task<BaseEditorResponseDto>> act = async () => await Subject.CreateDmo(DmoDto);
            var response = await act.Invoke();

            //Assert
            response.HttpCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        }

        [Fact]
        public async Task ShouldReturnCreatedDmoTest() {
            //Arrange
            SetMockAndVariables();
            var createdDmoDto = new CreatedDmoDto();

            EditorServiceMock.Setup(esm => esm.CreateAndLoadDmo(DmoDto, UserId)).ReturnsAsync(createdDmoDto);
            Subject = new EditorHub(
                EditorServiceMock.Object, 
                EnvironmentMock.Object, 
                ClaimsValidatorMock.Object,
                UserRepositoryMock.Object);
            SetupHubContext();

            //Act
            Func<Task<BaseEditorResponseDto>> act = async () => await Subject.CreateDmo(DmoDto);
            var response = await act.Invoke();

            //Assert
            response.Should().BeEquivalentTo(EditorResponseDto<CreatedDmoDto>.Ok(createdDmoDto));
        }

        [Fact]
        public async Task ShouldReturnInternalServerErrorResponseIfDmoWasNotCreatedTest() {
            //Arrange
            SetMockAndVariables();
            var exceptionMessage = "some message";
            
            EditorServiceMock.Setup(esm => esm.CreateAndLoadDmo(DmoDto, UserId))
                .ThrowsAsync(new CreateDmoException(exceptionMessage, new Exception("exception from repository")));
            Subject = new EditorHub(
                EditorServiceMock.Object,
                EnvironmentMock.Object,
                ClaimsValidatorMock.Object,
                UserRepositoryMock.Object);
            SetupHubContext();

            //Act
            Func<Task<BaseEditorResponseDto>> act = async () => await Subject.CreateDmo(DmoDto);
            var response = await act.Invoke();

            //Assert
            response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateInternalServerErrorResponse($"{CreateDmoException.CustomMessage} {exceptionMessage}"));
        }

        [Fact]
        public async Task ShouldReturnInternalServerErrorResponseIfDmoWasNotFoundAfterCreationTest() {
            //Arrange
            SetMockAndVariables();
            var exceptionMessage = "some message";

            EditorServiceMock.Setup(esm => esm.CreateAndLoadDmo(DmoDto, UserId))
                .ThrowsAsync(new LoadShortDmoException(exceptionMessage, new Exception("exception from repository")));
            Subject = new EditorHub(
                EditorServiceMock.Object, 
                EnvironmentMock.Object,
                ClaimsValidatorMock.Object,
                UserRepositoryMock.Object);
            SetupHubContext();

            //Act
            Func<Task<BaseEditorResponseDto>> act = async () => await Subject.CreateDmo(DmoDto);
            var response = await act.Invoke();

            //Assert
            response.Should().BeEquivalentTo(BaseEditorResponseDto.CreateInternalServerErrorResponse($"{LoadShortDmoException.CustomMessage} {exceptionMessage}"));
        }
    }
}
