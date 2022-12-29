using NNA.Domain.Entities;

namespace NNA.Domain.Interfaces.Repositories; 

public interface ICommunityRepository {
    Task<List<Dmo>> GetPublishedDmosAsync(int pageSize, int pageNumber, CancellationToken token);
    Task<int> GetPublishedAmountAsync(CancellationToken token);
    Task<int> GetPublishedDmosAmountAsync(List<Guid> dmoIdsToIgnore, string searchBy, CancellationToken token);

    Task<List<Dmo>> GetPublishedDmosAsync(List<Guid> dmoIdsToIgnore, string searchBy, int amount, CancellationToken token);

    Task<int> GetBeatsCount(Guid dmoId, CancellationToken token);
    Task<int> GetNonAestheticBeatsCount(Guid dmoId, CancellationToken token);

    Task<int> GetCharactersCount(Guid dmoId, CancellationToken token);
    Task<(string?, string?)> GetDmoPremiseAndControllingIdea(Guid dmoId, CancellationToken token);

}