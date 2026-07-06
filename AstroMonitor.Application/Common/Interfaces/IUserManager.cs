using AstroMonitor.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace AstroMonitor.Application.Common.Interfaces;

public interface IUserManager
{
    Task<ApplicationUser> FindByEmailAsync(string email);
    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
    Task CreateAsync(ApplicationUser user, string password);
    Task AddToRoleAsync(ApplicationUser user, string role);
    Task UpdateAsync(ApplicationUser user);
}