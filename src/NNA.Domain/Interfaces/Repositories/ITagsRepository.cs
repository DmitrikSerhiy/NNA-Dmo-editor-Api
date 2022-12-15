using NNA.Domain.Entities;

namespace NNA.Domain.Interfaces.Repositories;

public interface ITagsRepository
{
    Task<List<NnaTag>> GetAllTagsWithoutDescriptionAsync(CancellationToken token);
    Task<NnaTag?> GetTagAsync(Guid id, CancellationToken token);
}