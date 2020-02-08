using Microsoft.EntityFrameworkCore;
using Model;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    // ReSharper disable once UnusedMember.Global
    public class DmoCollectionRepository : IDmoCollectionRepository {

        private readonly NoNameContext _context;
        public DmoCollectionRepository(UnitOfWork unitOfWork) {
            if (unitOfWork == null) throw new ArgumentNullException(nameof(unitOfWork));

            _context = unitOfWork.Context;
        }

        public async Task<List<UserDmoCollection>> GetAllAsync(Guid userId) {
            return await _context.UserDmoCollections
                .Where(d => d.NoNameUserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<UserDmoCollection> Get(Guid collectionId, Guid userId) {
            return await _context.UserDmoCollections
                .FirstOrDefaultAsync(udc => udc.NoNameUserId == userId && udc.Id == collectionId);
        }

        public async Task<Boolean> IsExist(String collectionName, Guid userId) {
            return await _context.UserDmoCollections.AnyAsync(udc =>
                udc.CollectionName.Equals(collectionName, StringComparison.CurrentCultureIgnoreCase) && udc.NoNameUserId == userId);
        }

        public void Update(UserDmoCollection oldDmoCollection, UserDmoCollection newDmoCollection) {
            if (oldDmoCollection == null) throw new ArgumentNullException(nameof(oldDmoCollection));
            if (newDmoCollection == null) throw new ArgumentNullException(nameof(newDmoCollection));

            oldDmoCollection.CollectionName = newDmoCollection.CollectionName;
            _context.UserDmoCollections.Update(oldDmoCollection);
        }

        public async Task AddAsync(UserDmoCollection dmoCollection) {
            if (dmoCollection == null) throw new ArgumentNullException(nameof(dmoCollection));
            await _context.UserDmoCollections.AddAsync(dmoCollection);
        }

        public void Delete(UserDmoCollection collection) {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            _context.UserDmoCollections.Remove(collection);
        }

    }
}
