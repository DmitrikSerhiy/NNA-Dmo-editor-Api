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
    }
}
