using Microsoft.EntityFrameworkCore;
using NNA.Domain.Entities;
using NNA.Domain.Enums;
using NNA.Domain.Exceptions.Data;
using NNA.Domain.Interfaces;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Persistence.Repositories;

internal sealed class UserRepository : CommonRepository, IUserRepository {
    public UserRepository(IContextOrchestrator contextOrchestrator): base(contextOrchestrator) { }

    public void UpdateUser(NnaUser user) {
        Context.Set<NnaUser>().Attach(user);
        Context.Update(user);
    }

    public async Task<bool> HasEditorConnectionAsync(Guid userId, CancellationToken token) {
        return await Context.EditorConnections.AnyAsync(ec => ec.UserId == userId, token);
    }

    public void AddEditorConnection(EditorConnection connection) {
        Context.EditorConnections.Add(connection);
    }

    public async Task RemoveUserConnectionsAsync(Guid userId, CancellationToken token) {
        var connections = await Context.EditorConnections
            .Where(ec => ec.UserId == userId)
            .ToListAsync(token);

        if (connections.Count == 0) {
            return;
        }

        Context.EditorConnections.RemoveRange(connections);
    }

    public async Task<UsersTokens?> GetAuthenticatedUserDataAsync(string email, CancellationToken token) {
        return await Context.Set<UsersTokens>().SingleOrDefaultAsync(ut => ut.Email == email, token);
    }

    public void SaveTokens(NnaToken accessToken, NnaToken refreshToken) {
        if (accessToken == null) throw new ArgumentNullException(nameof(accessToken));
        if (refreshToken == null) throw new ArgumentNullException(nameof(refreshToken));

        Context.AddRange(accessToken, refreshToken);
    }

    public async Task ClearTokensAsync(NnaUser user, CancellationToken token) {
        if (user == null) throw new ArgumentNullException(nameof(user));
        var tokens = await Context.Tokens.Where(tkn => tkn.UserId == user.Id).ToListAsync(token);
        Context.Tokens.RemoveRange(tokens);
    }

    public void UpdateTokens(NnaToken accessToken, NnaToken refreshToken) {
        if (accessToken == null) throw new ArgumentNullException(nameof(accessToken));
        if (refreshToken == null) throw new ArgumentNullException(nameof(refreshToken));

        Context.AttachRange(accessToken, refreshToken);
        Context.Tokens.UpdateRange(accessToken, refreshToken);
    }

    public void ConfirmUserEmail(NnaUser user) {
        if (user == null) throw new ArgumentNullException(nameof(user));
        Context.Attach(user);
        user.EmailConfirmed = true;
        Context.Update(user);
    }

    public async Task<(NnaToken accessToken, NnaToken refreshToken)?> GetTokens(Guid userId, CancellationToken token) {
        var tokens = await Context.Tokens
            .Where(tkn => tkn.UserId == userId)
            .AsNoTracking()
            .ToListAsync(token);

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

    public async Task<NnaUser> FirstUserAsync(CancellationToken token) {
        return await Context.ApplicationUsers.FirstAsync(token);
    }

    public async Task<NnaUser?> WithIdAsync(Guid id, CancellationToken token) {
        return await Context.ApplicationUsers.FirstOrDefaultAsync(user => user.Id == id, token);
    }

    // do not use it in normal app lifecycle
    public void SanitiseEditorConnections() {
        var editorConnections = Context.EditorConnections.ToList();
        Context.RemoveRange(editorConnections);
        Context.SaveChanges();
    }

    public void SanitiseUserTokens() {
        var tokens = Context.Tokens.ToList();
        Context.RemoveRange(tokens);
        Context.SaveChanges();
    }
}