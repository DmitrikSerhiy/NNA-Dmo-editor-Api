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
                .OrderByDescending(d => d.DateOfCreation)
                .ToListAsync();
        }

        public async Task<UserDmoCollection> GetCollectionAsync(Guid userId, Guid? collectionId) {
            if(!collectionId.HasValue) throw new ArgumentNullException(nameof(collectionId));
            return await _context.UserDmoCollections
                .FirstOrDefaultAsync(udc => udc.Id == collectionId && udc.NoNameUserId == userId);
        }

        public async Task<Dmo> GetDmoAsync(Guid userId, Guid? dmoId) {
            if (!dmoId.HasValue) throw new ArgumentNullException(nameof(dmoId));
            return await _context.Dmos.FirstOrDefaultAsync(d => d.Id == dmoId && d.NoNameUserId == userId);
        }

        public async Task<Boolean> IsCollectionExist(Guid userId, String collectionName) {
            if(String.IsNullOrWhiteSpace(collectionName)) throw new ArgumentNullException(nameof(collectionName));
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
            collection.DmoUserDmoCollections = null;
            _context.UserDmoCollections.Remove(collection);
        }

        public async Task<UserDmoCollection> GetCollectionWithDmos(Guid userId, Guid? collectionId) {
            if (!collectionId.HasValue) throw new ArgumentNullException(nameof(collectionId));
            var dmoCollection = await _context.UserDmoCollections
                .Where(d => d.NoNameUserId == userId && d.Id == collectionId)
                .Include(dc => dc.DmoUserDmoCollections)
                    .ThenInclude(d => d.Dmo)
                .FirstOrDefaultAsync();

            dmoCollection.DmoUserDmoCollections = dmoCollection.DmoUserDmoCollections.OrderByDescending(d => d.Dmo.DateOfCreation).ToList();
            return dmoCollection;
        }

        public void AddDmoToCollection(UserDmoCollection dmoCollection, Dmo dmo) {
            if (dmoCollection == null) throw new ArgumentNullException(nameof(dmoCollection));
            if (dmo == null) throw new ArgumentNullException(nameof(dmo));
            dmoCollection.DmoUserDmoCollections.Add(new DmoUserDmoCollection
            {
                DmoId = dmo.Id,
                Dmo = dmo,
                UserDmoCollection = dmoCollection,
                UserDmoCollectionId = dmoCollection.Id
            });
        }

        public Boolean ContainsDmo(UserDmoCollection dmoCollection, Guid? dmoId) {
            if (dmoCollection == null) throw new ArgumentNullException(nameof(dmoCollection));
            if (!dmoId.HasValue) throw new ArgumentNullException(nameof(dmoId));

            return dmoCollection.DmoUserDmoCollections.Any(d => d.DmoId == dmoId);
        }

        public void RemoveDmoFromCollection(UserDmoCollection dmoCollection, Dmo dmo) {
            if (dmoCollection == null) throw new ArgumentNullException(nameof(dmoCollection));
            if (dmo == null) throw new ArgumentNullException(nameof(dmo));

            var dmod = dmo.DmoUserDmoCollections.First(d => d.DmoId == dmo.Id);
            dmod.UserDmoCollection = null;
        }

    }
}
