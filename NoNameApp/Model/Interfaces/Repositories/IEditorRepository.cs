using System;
using System.Threading.Tasks;
using Model.Entities;
using Model.Enums;

namespace Model.Interfaces.Repositories {
    public interface IEditorRepository {
        Task<BeatUpdateStatus> UpdateBeatsAsync(string jsonBeats, Guid dmoId);
        Task<Dmo> LoadShortDmoAsync(Guid dmoId, Guid userId);
        Task<bool> CreateDmoAsync(Dmo dmo);
        Task<Dmo> EditDmoAsync(Dmo dmo, Guid userId);
    }
}
