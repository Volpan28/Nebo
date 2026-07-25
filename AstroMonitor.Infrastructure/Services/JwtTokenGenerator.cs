using System.Security.Cryptography;
using System.Text;
using AstroMonitor.Application.Common.Interfaces;
using AstroMonitor.Application.Features.Auth;
using AstroMonitor.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AstroMonitor.Infrastructure.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _options;

    public JwtTokenGenerator(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }
    
    public TokenResponse GenerateToken(string userId, string email, IList<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new Dictionary<string, object>
        {
            { JwtRegisteredClaimNames.Sub, userId },
            { JwtRegisteredClaimNames.Email, email },
            { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() },

        };

        if (roles is { Count: > 0 })
        {
            claims.Add("roles", roles);
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Claims = claims,
            Expires = DateTime.UtcNow.AddMinutes(_options.AccessExpiryMinutes),
            SigningCredentials = credentials,
        };

        var handler = new JsonWebTokenHandler();
        string accessToken = handler.CreateToken(tokenDescriptor);

        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        string refreshToken = Convert.ToBase64String(randomNumber);

        DateTime refreshTokenExpiresAt = DateTime.UtcNow.AddDays(_options.RefreshExpiryDays);
        
        return new TokenResponse(accessToken, refreshToken, refreshTokenExpiresAt);
    }
}