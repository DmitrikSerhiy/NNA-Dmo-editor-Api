using NNA.Domain.Entities;

namespace NNA.Domain.Interfaces.Repositories;

public interface IUserRepository : IRepository {
    Task<NnaUser?> WithIdAsync(Guid id, CancellationToken token);
    void UpdateUser(NnaUser user);
    Task<NnaUser> FirstUserAsync(CancellationToken token);
    void SaveTokens(NnaToken accessToken, NnaToken refreshToken);
    Task ClearTokensAsync(NnaUser user, CancellationToken token);
    void UpdateTokens(NnaToken accessToken, NnaToken refreshToken);
    void ConfirmUserEmail(NnaUser user);
    Task<(NnaToken accessToken, NnaToken refreshToken)?> GetTokens(Guid userId, CancellationToken token);
    Task<UsersTokens?> GetAuthenticatedUserDataAsync(string email, CancellationToken token);
    Task<bool> HasEditorConnectionAsync(Guid userId, CancellationToken token);
    void AddEditorConnection(EditorConnection connection);
    void RemoveEditorConnection(EditorConnection connection);
    Task RemoveEditorConnectionOnLogout(Guid userId, CancellationToken token);
    void SanitiseEditorConnections();
    void SanitiseUserTokens();
}