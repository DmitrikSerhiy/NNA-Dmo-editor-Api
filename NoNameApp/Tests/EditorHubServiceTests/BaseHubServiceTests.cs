using API.Features.Editor.Services;
using AutoMapper;
using Model.Interfaces.Repositories;
using Moq;

namespace Tests.EditorHubServiceTests {
    public abstract class BaseHubServiceTests {

        protected Mock<IEditorRepository> RepositoryMock { get; private set; }
        protected Mock<IMapper> MapperMock { get; private set; }
        protected EditorService Subject { get; set; }


        protected void SetupConstructorMocks() {
            RepositoryMock = new Mock<IEditorRepository>();
            MapperMock = new Mock<IMapper>();
        }

    }
}
