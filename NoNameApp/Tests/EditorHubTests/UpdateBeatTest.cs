using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Features.Editor.Hubs;
using Microsoft.AspNetCore.SignalR;
using Model.DTOs.Editor;
using Model.Exceptions.Editor;
using Moq;
using Xunit;

namespace Tests.EditorHubTests {
    public class UpdateBeatTest: BaseEditorTests {
        private UpdateBeatDto BeatDto { get; set; }
        
        private void SetMockAndVariables()
        {
            SetupConstructorMocks();
            BeatDto = new UpdateBeatDto {
                BeatId = Guid.NewGuid().ToString(),
                Text = "Beat description",
                Time = new UpdateBeatTimeDto {
                    Hours = 0,
                    Minutes = 10,
                    Seconds = 10
                }
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
            Func<Task> act = async () => await Subject.UpdateBeat(null);
            await act.Invoke();

            //Assert
            EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
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
            SetupHubContext();
            var hubContext = new Mock<HubCallerContext>();
            hubContext.Setup(hm => hm.Items).Returns(new Dictionary<object, object>());
            Subject.Context = hubContext.Object;
        
            //Act
            Func<Task> act = async () => await Subject.UpdateBeat(BeatDto);
            await act.Invoke();

            //Assert
            EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
        }
        
        
        [Fact]
        public async Task ShouldReturnNotValidResponseIfDtoIsNotValidTest() {
            //Arrange
            SetMockAndVariables();
            BeatDto.BeatId = null;
            Subject = new EditorHub(
                EditorServiceMock.Object, 
                EnvironmentMock.Object,
                ClaimsValidatorMock.Object,
                UserRepositoryMock.Object);
            SetupHubContext();
        
            //Act
            Func<Task> act = async () => await Subject.UpdateBeat(BeatDto);
            await act.Invoke();
        
            //Assert
            EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
        }
        
        
        [Fact]
        public async Task ShouldReturnNoContentResponseTest() {
            //Arrange
            SetMockAndVariables();
        
            EditorServiceMock.Setup(esm => esm.UpdateBeat(BeatDto, UserId)).Verifiable();
            Subject = new EditorHub(
                EditorServiceMock.Object, 
                EnvironmentMock.Object,
                ClaimsValidatorMock.Object,
                UserRepositoryMock.Object);
            SetupHubContext();
        
            //Act
            Func<Task> act = async () => await Subject.UpdateBeat(BeatDto);
            await act.Invoke();
        
            //Assert
            EditorServiceMock.Verify(esm => esm.UpdateBeat(BeatDto, UserId), Times.Once);
            EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Never());
        }
        
                
        [Fact]
        public async Task ShouldReturnInternalServerErrorResponseIfRepositoryThrowsTest() {
            //Arrange
            SetMockAndVariables();
            var exceptionMessage = "some message";
        
            EditorServiceMock.Setup(esm => esm.UpdateBeat(BeatDto, UserId))
                .ThrowsAsync(new UpdateBeatException(exceptionMessage, new Exception("exception from repository")));
            Subject = new EditorHub(
                EditorServiceMock.Object, 
                EnvironmentMock.Object,
                ClaimsValidatorMock.Object,
                UserRepositoryMock.Object);
            SetupHubContext();
        
            //Act
            Func<Task> act = async () => await Subject.UpdateBeat(BeatDto);
            await act.Invoke();
        
            //Assert
            EditorClientsMock.Verify(sbj => sbj.Caller.OnServerError(It.IsAny<object>()), Times.Once());
        }
        
    }
}