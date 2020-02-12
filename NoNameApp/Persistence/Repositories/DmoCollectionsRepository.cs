using Microsoft.EntityFrameworkCore;
using Model;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence.Repositories {
    // ReSharper disable once UnusedMember.Global
    public class DmoCollectionsRepository : IDmoCollectionsRepository {

        private readonly NoNameContext _context;
        public DmoCollectionsRepository(UnitOfWork unitOfWork) {
            if (unitOfWork == null) throw new ArgumentNullException(nameof(unitOfWork));

            _context = unitOfWork.Context;
        }

        public async Task<List<UserDmoCollection>> GetCollectionsAsync(Guid userId) {
            return await _context.UserDmoCollections
                .Where(d => d.NoNameUserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<UserDmoCollection> GetCollection(Guid collectionId, Guid userId) {
            return await _context.UserDmoCollections
                .FirstOrDefaultAsync(udc => udc.NoNameUserId == userId && udc.Id == collectionId);
        }

        public async Task<Dmo> GetDmoAsync(Guid userId, Guid dmoId) {
            return await _context.Dmos.FirstOrDefaultAsync(d => d.Id == dmoId && d.NoNameUserId == userId);
        }

        public async Task<Boolean> IsCollectionExist(String collectionName, Guid userId) {
            return await _context.UserDmoCollections.AnyAsync(udc =>
                udc.CollectionName.Equals(collectionName, StringComparison.CurrentCultureIgnoreCase) && udc.NoNameUserId == userId);
        }

        public void UpdateCollectionName(UserDmoCollection oldDmoCollection, UserDmoCollection newDmoCollection) {
            if (oldDmoCollection == null) throw new ArgumentNullException(nameof(oldDmoCollection));
            if (newDmoCollection == null) throw new ArgumentNullException(nameof(newDmoCollection));

            oldDmoCollection.CollectionName = newDmoCollection.CollectionName;
            _context.UserDmoCollections.Update(oldDmoCollection);
        }

        public async Task AddCollectionAsync(UserDmoCollection dmoCollection) {
            if (dmoCollection == null) throw new ArgumentNullException(nameof(dmoCollection));
            await _context.UserDmoCollections.AddAsync(dmoCollection);
        }

        public void DeleteCollection(UserDmoCollection collection) {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            _context.UserDmoCollections.Remove(collection);
        }

        public async Task<UserDmoCollection> GetCollectionWithDmos(Guid userId, Guid collectionId) {
            return await _context.UserDmoCollections
                .Where(d => d.NoNameUserId == userId && d.Id == collectionId)
                .Include(dc => dc.Dmos)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public void DeleteDmoFromCollection(Dmo dmo) {
            if (dmo == null) throw new ArgumentNullException(nameof(dmo));
            dmo.UserDmoCollection = null;
        }

    }
}
