using Model.DTOs.Editor;
using System;
using System.Threading.Tasks;

namespace Model.Interfaces {
    public interface IEditorService {
        Task<CreatedDmoDto> CreateAndLoadDmo(CreateDmoDto dto, Guid userId);
        Task UpdateShortDmo(UpdateShortDmoDto dmoDto, Guid userId);
        Task<LoadedShortDmoDto> LoadShortDmo(LoadShortDmoDto dmoDto, Guid userId);
        Task UpdateDmoBeatsAsJson(UpdateDmoBeatsAsJsonDto dmoDto, Guid userId);
        Task SetBeatsId(SetBeatsIdDto dmoDto, Guid userId);
    }
}
