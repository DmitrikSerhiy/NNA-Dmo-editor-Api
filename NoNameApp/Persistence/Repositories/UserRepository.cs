using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Model.Interfaces.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Model.Enums;
using Model.Exceptions.Data;

namespace Persistence.Repositories {
    // ReSharper disable once UnusedMember.Global
    internal sealed class UserRepository : IUserRepository {
        private readonly NnaContext _context;
        public UserRepository(UnitOfWork unitOfWork) {
            if (unitOfWork == null) throw new ArgumentNullException(nameof(unitOfWork));

            _context = unitOfWork.Context;
        }

        public async Task<UsersTokens> GetAuthenticatedUserDataAsync(string email) {
            return await _context.Set<UsersTokens>().SingleOrDefaultAsync(ut => ut.Email == email);
        }

        public async Task SaveTokens(NnaToken accessToken, NnaToken refreshToken) {
            if (accessToken == null) throw new ArgumentNullException(nameof(accessToken));
            if (refreshToken == null) throw new ArgumentNullException(nameof(refreshToken));
            
            await _context.AddRangeAsync(accessToken, refreshToken);
        }

        public async Task ClearTokens(NnaUser user) {
            if (user == null) throw new ArgumentNullException(nameof(user));
            var tokens = await _context.Tokens.Where(token => token.UserId == user.Id).ToListAsync();
            _context.Tokens.RemoveRange(tokens);
        }

        public void UpdateTokens(NnaToken accessToken, NnaToken refreshToken) {
            if (accessToken == null) throw new ArgumentNullException(nameof(accessToken));
            if (refreshToken == null) throw new ArgumentNullException(nameof(refreshToken));
            
            _context.AttachRange(accessToken, refreshToken);
            _context.Tokens.UpdateRange(accessToken, refreshToken);
        }

        public async Task<(NnaToken accessToken, NnaToken refreshToken)?> GetTokens(Guid userId) {
            var tokens = await _context.Tokens
                .Where(token => token.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
            
            if (tokens.Count != 0 && tokens.Count != 2) {
                throw new InconsistentDataException(
                    $"Expected to get 2 tokens [access and refresh] for user {userId}. But found: {tokens.Count} amount");
            }

            if (tokens.Count == 0) {
                return null;
            }
            
            return tokens[0].Name == nameof(TokenName.Access) 
                ? (accessToken: tokens[0], refreshToken: tokens[1]) 
                : (accessToken: tokens[1], refreshToken: tokens[0]);
        }

        public async Task<NnaUser> FirstUser() {
            return await _context.ApplicationUsers.FirstAsync();
        }

        public async Task<NnaUser> WithId(Guid id) {
            return await _context.ApplicationUsers.FindAsync(id);
        }
    }
}
