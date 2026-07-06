namespace AstroMonitor.Application.Features.Auth;

public record TokenResponse(string AccessToken, string RefreshToken, DateTime RefreshTokenExpiresAt);