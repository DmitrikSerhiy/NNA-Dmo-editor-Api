﻿using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Entities;

namespace Persistence.Repositories {
    // ReSharper disable once UnusedMember.Global
    public class UserRepository : IUserRepository
    {
        private readonly NnaContext _context;
        public UserRepository(UnitOfWork unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException(nameof(unitOfWork));

            _context = unitOfWork.Context;
        }

        public async Task<NnaUser> FirstUser() {
            return await _context.ApplicationUsers.FirstAsync();
        }
        public async Task<NnaUser> WithId(Guid id) {
            return await _context.ApplicationUsers.FindAsync(id);
        }

    }
}
