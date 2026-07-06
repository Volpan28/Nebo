using AstroMonitor.Application.Common.Exceptions;
using AstroMonitor.Application.Common.Interfaces;
using MediatR;

namespace AstroMonitor.Application.Features.Auth.Commands.Login;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, TokenResponse>
{
    private readonly IUserManager _userManager;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public LoginUserCommandHandler(IUserManager userManager, IJwtTokenGenerator tokenGenerator)
    {
        _userManager = userManager;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<TokenResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            throw new InvalidCredentialsExceptions();
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            throw new InvalidCredentialsExceptions();
        }
        
        var roles = await _userManager.GetRolesAsync(user);
        
        var tokenResponse = _tokenGenerator.GenerateToken(user.Id, user.Email, roles);
        
        user.LastLoginDate = DateTimeOffset.UtcNow;
        user.RefreshToken = tokenResponse.RefreshToken;
        user.RefreshTokenExpiryTime = tokenResponse.RefreshTokenExpiresAt;

        await _userManager.UpdateAsync(user);
        return tokenResponse;
    }
}