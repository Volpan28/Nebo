using AstroMonitor.Application.Features.Auth;

namespace AstroMonitor.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    TokenResponse GenerateToken(string userId, string email, IList<string> roles);
}