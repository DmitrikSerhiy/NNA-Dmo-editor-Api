using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Entities;

namespace Persistence.Repositories {
    // ReSharper disable once UnusedMember.Global
    public class DmoCollectionRepository : IDmoCollectionRepository {
        
        private readonly NoNameContext _context;
        public DmoCollectionRepository(UnitOfWork unitOfWork) {
            if (unitOfWork == null) throw new ArgumentNullException(nameof(unitOfWork));

            _context = unitOfWork.Context;
        }

        public async Task<List<UserDmoCollection>> GetAllDmoAsync(Guid userId, Guid collectionId) {
            return await _context.UserDmoCollections
                .Where(d => d.NoNameUserId == userId && d.Id == collectionId)
                .Include(dc => dc.Dmos)
                .AsNoTracking()
                .ToListAsync();
        }

    }
}
