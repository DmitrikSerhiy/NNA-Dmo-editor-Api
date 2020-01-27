using Microsoft.EntityFrameworkCore;
using Model;
using Model.Entities;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class DmoRepository : IDmoRepository {

        private readonly NoNameContext _context;
        public DmoRepository(UnitOfWork unitOfWork) {
            if (unitOfWork == null) throw new ArgumentNullException(nameof(unitOfWork));

            _context = unitOfWork.Context;
        }

        public async Task<ImmutableList<Dmo>> GetAll(Guid userId) {
            var dmos = await _context.Dmos
                .Where(d => d.NoNameUserId == userId)
                .AsNoTracking()
                .ToArrayAsync();
            
            return ImmutableList.Create(dmos);
        }

        public async Task Add(Dmo dmo) {
            if (dmo == null) throw new ArgumentNullException(nameof(dmo));
            await _context.Dmos.AddAsync(dmo);
        }

    }
}
