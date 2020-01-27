using System;
using System.Threading.Tasks;
using Model;
using Model.Entities;

namespace Persistence.Repositories {
    public class UserRepository : IUserRepository
    {
        private readonly NoNameContext _context;
        public UserRepository(UnitOfWork unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException(nameof(unitOfWork));

            _context = unitOfWork.Context;
        }

        //temporary
        public async Task<NoNameUser> WithId(Guid id) {
            return await _context.ApplicationUsers.FindAsync(id);
        }

    }
}
