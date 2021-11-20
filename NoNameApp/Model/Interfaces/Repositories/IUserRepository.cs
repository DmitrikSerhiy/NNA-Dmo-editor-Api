using System;
using System.Threading.Tasks;
using Model.DTOs.Account;
using Model.Entities;

namespace Model.Interfaces.Repositories {
    public interface IUserRepository {
        Task<NnaUser> WithId(Guid id);
        Task<NnaUser> FirstUser();
        Task SaveTokens(NnaToken accessToken, NnaToken refreshToken);
        Task ClearTokens(NnaUser user);
        void UpdateTokens(NnaToken accessToken, NnaToken refreshToken);
        Task<(NnaToken accessToken, NnaToken refreshToken)?> GetTokens(Guid userId);
        Task<UsersTokens> GetAuthenticatedUserDataAsync(string email);
    }
}
