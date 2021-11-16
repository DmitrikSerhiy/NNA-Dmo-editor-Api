using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Model.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model.DTOs.Account;

namespace Persistence.Repositories {
    // ReSharper disable once UnusedMember.Global
    internal sealed class UserRepository : IUserRepository {
        private readonly NnaContext _context;
        public UserRepository(UnitOfWork unitOfWork) {
            if (unitOfWork == null) throw new ArgumentNullException(nameof(unitOfWork));

            _context = unitOfWork.Context;
        }

        public async Task<bool> IsAccessTokenExist(Guid userId, string accessTokenId) {
            return await _context.Logins.AnyAsync(login => login.UserId == userId && login.AccessTokenId == accessTokenId);
        }
        
        public async Task<bool> IsRefreshTokenExist(Guid userId, string refreshTokenId) {
            return await _context.Logins.AnyAsync(login => login.UserId == userId && login.RefreshTokenId == refreshTokenId);
        }
        
        // todo: aspnettokens table instead
        public async Task UpdateLogin(Guid userId, TokensDto tokensDto) {
            // it's important so single user always get same token
            var login = await _context.Logins.SingleAsync(login => login.UserId == userId);
            _context.Entry(login).State = EntityState.Deleted;
            await _context.AddAsync(new NnaLogin {
                LoginProvider = "password",
                ProviderKey = DateTime.UtcNow.Millisecond.ToString(),
                UserId = userId,
                ProviderDisplayName = "Login with password",
                AccessTokenId = tokensDto.AccessToken,
                RefreshTokenId = tokensDto.RefreshToken
            });
        }

        public async Task<NnaUser> FirstUser() {
            return await _context.ApplicationUsers.FirstAsync();
        }

        public async Task<NnaUser> WithId(Guid id) {
            return await _context.ApplicationUsers.FindAsync(id);
        }
    }
}
