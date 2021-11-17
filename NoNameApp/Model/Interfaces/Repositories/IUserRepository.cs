using System;
using System.Threading.Tasks;
using Model.DTOs.Account;
using Model.Entities;

namespace Model.Interfaces.Repositories {
    public interface IUserRepository {
        Task<NnaUser> WithId(Guid id);
        Task<NnaUser> FirstUser();

        Task<bool> IsAccessTokenExist(Guid userId, string accessTokenId, string loginProvider);
        Task<bool> IsRefreshTokenExist(Guid userId, string refreshTokenId, string loginProvider);
        Task SaveTokens(NnaToken accessToken, NnaToken refreshToken);
        void UpdateTokens(NnaToken accessToken, NnaToken refreshToken);
        Task<(NnaToken accessToken, NnaToken refreshToken)?> GetTokens(Guid userId);
    }
}
