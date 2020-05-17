using System;
using System.Threading.Tasks;
using Model.Entities;
using Model.Enums;

namespace Model {
    public interface IBeatsRepository {
        Task<BeatUpdateStatus> UpdateBeatsAsync(string jsonBeats, Guid dmoId);
        Task<Dmo> LoadDmoAsync(Guid dmoId, Guid userId);
        Task<Dmo> CreateDmoAsync(Dmo dmoFromClient, Guid userId);
        Task<Dmo> EditDmoAsync(Dmo dmoFromClient, Guid userId);
    }
}
