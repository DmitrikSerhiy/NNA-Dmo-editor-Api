using NNA.Domain.Entities;

namespace NNA.Domain.Interfaces.Repositories; 

public interface ICommunityRepository {
    Task<List<Dmo>> GetPublishedDmosAsync(int pageSize, int pageNumber, CancellationToken token);
    Task<int> GetPublishedAmountAsync(CancellationToken token);

}