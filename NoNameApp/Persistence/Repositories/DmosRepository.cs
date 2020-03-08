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
    public class DmosRepository : IDmosRepository {

        private readonly NoNameContext _context;
        public DmosRepository(UnitOfWork unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException(nameof(unitOfWork));

            _context = unitOfWork.Context;
        }

        public async Task<List<Dmo>> GetAll(Guid userId) {
            return await _context.Dmos.Where(d => d.NoNameUserId == userId)
                .OrderByDescending(d => d.DateOfCreation)
                .ToListAsync();
        }

        public async Task<Dmo> GetDmo(Guid userId, Guid? dmoId) {
            if (!dmoId.HasValue) throw new ArgumentNullException(nameof(dmoId));
            return await _context.Dmos.FirstOrDefaultAsync(d => d.NoNameUserId == userId && d.Id == dmoId.Value);
        }

        public void RemoveDmo(Dmo dmo) {
            if (dmo == null) throw new ArgumentNullException(nameof(dmo));

            _context.Dmos.Remove(dmo);
        }
    }
}
