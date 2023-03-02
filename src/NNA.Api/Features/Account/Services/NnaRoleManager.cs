using Microsoft.AspNetCore.Identity;
using NNA.Domain.Enums;

namespace NNA.Api.Features.Account.Services; 

public sealed class NnaRoleManager: RoleManager<IdentityRole<Guid>> {
    
    public NnaRoleManager(
        IRoleStore<IdentityRole<Guid>> store, 
        IEnumerable<IRoleValidator<IdentityRole<Guid>>> roleValidators, 
        ILookupNormalizer keyNormalizer, 
        IdentityErrorDescriber errors, 
        ILogger<RoleManager<IdentityRole<Guid>>> logger) 
        : base(store, roleValidators, keyNormalizer, errors, logger) {
    }

    public string GetInitialRole() {
        return Enum.GetName(NnaRoles.NotActiveUser)!;
    }

    public string GetActiveUserRole() {
        return Enum.GetName(NnaRoles.ActiveUser)!;
    }

    public string GetSuperUserRole() {
        return Enum.GetName(NnaRoles.SuperUser)!;
    }
}