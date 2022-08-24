using AutoMapper;
using Moq;
using NNA.Api.Features.Editor.Services;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Tests.EditorHubServiceTests; 
public abstract class BaseHubServiceTests {
    protected Mock<IEditorRepository> RepositoryMock { get; private set; }
    protected Mock<IMapper> MapperMock { get; private set; }
    protected EditorService Subject { get; set; }
    
    protected void SetupConstructorMocks() {
        RepositoryMock = new Mock<IEditorRepository>();
        MapperMock = new Mock<IMapper>();
    }

}