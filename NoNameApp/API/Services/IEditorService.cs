using System;
using System.Threading.Tasks;
using API.DTO.Dmos;

namespace API.Services
{
    public interface IEditorService {
        Task<ShortDmoDto> CreateAndLoadAsync(ShortDmoDto dto, Guid userId);
    }
}
