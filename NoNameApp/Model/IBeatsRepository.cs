using System;
using System.Threading.Tasks;
using Model.Entities;

namespace Model {
    public interface IBeatsRepository {
        Task<BeatUpdateStatus> UpdateBeatsAsync(string jsonBeats, Guid dmoId);
        Task<Dmo> LoadDmoAsync(Guid dmoId, Guid userId);
    }
}
