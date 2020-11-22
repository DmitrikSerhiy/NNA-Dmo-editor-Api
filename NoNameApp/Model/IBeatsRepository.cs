using System;
using System.Threading.Tasks;
using Model.Entities;
using Model.Enums;

namespace Model {
    public interface IBeatsRepository {
        Task<BeatUpdateStatus> UpdateBeatsAsync(string jsonBeats, Guid dmoId);
        Task<Dmo> LoadShortDmoAsync(Guid dmoId, Guid userId);
        Task<bool> CreateDmoAsync(Dmo dmo, Guid userId);
        Task<Dmo> EditDmoAsync(Dmo dmo, Guid userId);
    }
}
