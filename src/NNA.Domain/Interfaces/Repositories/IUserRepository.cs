﻿using NNA.Domain.Entities;

namespace NNA.Domain.Interfaces.Repositories; 
public interface IUserRepository: IRepository {
    Task<NnaUser?> WithId(Guid id);
    void UpdateUser(NnaUser user);
    Task<NnaUser> FirstUser();
    Task SaveTokens(NnaToken accessToken, NnaToken refreshToken);
    Task ClearTokens(NnaUser user);
    void UpdateTokens(NnaToken accessToken, NnaToken refreshToken);
    void ConfirmUserEmail(NnaUser user);
    Task<(NnaToken accessToken, NnaToken refreshToken)?> GetTokens(Guid userId);
    Task<UsersTokens?> GetAuthenticatedUserDataAsync(string email);
    Task<bool> HasEditorConnectionAsync(Guid userId);
    Task AddEditorConnectionAsync(EditorConnection connection);
    void RemoveEditorConnection(EditorConnection connection);
    Task RemoveEditorConnectionOnLogout(Guid userId);
    Task SyncContextImmediatelyAsync();
    void SanitiseEditorConnections();
    void SanitiseUserTokens();
}