using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Model.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence.Repositories {
    // ReSharper disable once UnusedMember.Global
    public class DmosRepository : IDmosRepository {

        private readonly NnaContext _context;
        public DmosRepository(UnitOfWork unitOfWork) {
            if (unitOfWork == null) throw new ArgumentNullException(nameof(unitOfWork));

            _context = unitOfWork.Context;
        }

        public async Task<List<Dmo>> GetAll(Guid userId) {
            return await _context.Dmos.Where(d => d.NnaUserId == userId)
                .OrderByDescending(d => d.DateOfCreation)
                .ToListAsync();
        }

        public async Task<Dmo> GetShortDmo(Guid userId, Guid? dmoId) {
            if (!dmoId.HasValue) throw new ArgumentNullException(nameof(dmoId));
            return await _context.Dmos.FirstOrDefaultAsync(d => d.NnaUserId == userId && d.Id == dmoId.Value);
        }


        public async Task<Dmo> GetDmo(Guid userId, Guid? dmoId) {
            if (!dmoId.HasValue) throw new ArgumentNullException(nameof(dmoId));
            return await _context.Dmos
                .Include(d => d.DmoCollectionDmos)
                    .ThenInclude(dd => dd.DmoCollection)
                .FirstOrDefaultAsync(d => d.NnaUserId == userId && d.Id == dmoId.Value);
        }

        public void DeleteDmo(Dmo dmo) {
            if (dmo == null) throw new ArgumentNullException(nameof(dmo));
            dmo.DmoCollectionDmos.Clear();
            _context.Dmos.Remove(dmo);
        }

        public async Task<Dmo> GetDmoWithBeatsJson(Guid userId, Guid dmoId) {
            return await _context.Dmos
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.NnaUserId == userId && d.Id == dmoId);
        }
    }
}
