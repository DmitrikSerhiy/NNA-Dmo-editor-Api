using Microsoft.EntityFrameworkCore;
using NNA.Domain.Entities;
using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Persistence.Repositories;

internal sealed class TagsRepository : CommonRepository, ITagsRepository
{
    public TagsRepository(IContextOrchestrator contextOrchestrator) : base(contextOrchestrator) {}

    public async Task<List<NnaTag>> GetAllTagsWithoutDescriptionAsync(CancellationToken token)
    {
        var nnaTags = await Context.Tags
            .Select(tag => new { tag.Name, tag.Id})
            .ToListAsync(token);

        return nnaTags.Select(tag =>
        {
            var newTag = new NnaTag
            {
                Name = tag.Name
            };
            newTag.SetIdExplicitly(tag.Id);
            return newTag;
        }).ToList();
    }

    public async Task<NnaTag?> GetTagAsync(Guid id, CancellationToken token)
    {
        return await Context.Tags.FirstOrDefaultAsync(tag => tag.Id == id, token);
    }
}