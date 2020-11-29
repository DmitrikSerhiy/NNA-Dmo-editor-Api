using System;
using System.Threading.Tasks;
using Model.DTOs.Editor;

namespace Model.Interfaces
{
    public interface IEditorService {
        Task<CreatedDmoDto> CreateAndLoadAsync(CreateDmoDto dto, Guid userId);
    }
}
