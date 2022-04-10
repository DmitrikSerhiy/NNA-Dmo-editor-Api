using System;
using System.Threading.Tasks;
using Model.Entities;

namespace Model.Interfaces.Repositories {
    public interface IUserRepository {
        Task<NnaUser> WithId(Guid id);
        void UpdateUser(NnaUser user);
        Task<NnaUser> FirstUser();
        Task SaveTokens(NnaToken accessToken, NnaToken refreshToken);
        Task ClearTokens(NnaUser user);
        void UpdateTokens(NnaToken accessToken, NnaToken refreshToken);
        void ConfirmUserEmail(NnaUser user);
        Task<(NnaToken accessToken, NnaToken refreshToken)?> GetTokens(Guid userId);
        Task<UsersTokens> GetAuthenticatedUserDataAsync(string email);
        Task<bool> HasEditorConnectionAsync(Guid userId);
        Task AddEditorConnectionAsync(EditorConnection connection);
        void RemoveEditorConnection(EditorConnection connection);
        Task RemoveEditorConnectionOnLogout(Guid userId);
        Task SyncContextImmediatelyAsync();
    }
}
