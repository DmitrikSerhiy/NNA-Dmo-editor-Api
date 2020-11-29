using System;
using System.Threading.Tasks;
using Model.DTOs.Dmos;

namespace Model.Interfaces
{
    public interface IEditorService {
        Task<ShortDmoDto> CreateAndLoadAsync(ShortDmoDto dto, Guid userId);
    }
}
