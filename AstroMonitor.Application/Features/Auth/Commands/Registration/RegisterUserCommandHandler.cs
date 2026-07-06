using AstroMonitor.Application.Common.Interfaces;
using AstroMonitor.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AstroMonitor.Application.Features.Auth.Commands.Registration;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, TokenResponse>
{
    private readonly IUserManager _userManager;
    private readonly IJwtTokenGenerator _tokenGenerator;
    
    public RegisterUserCommandHandler(IUserManager userManager, IJwtTokenGenerator tokenGenerator)
    {
        _userManager = userManager;
        _tokenGenerator = tokenGenerator;
    }
    
    public async Task<TokenResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            RegistrationDate = DateTimeOffset.UtcNow,
            LastLoginDate = DateTimeOffset.UtcNow
        };

        await _userManager.CreateAsync(user, request.Password);
        await _userManager.AddToRoleAsync(user, UserRoles.User);
        
        var roles = new List<string> { UserRoles.User };
        var tokenResponse = _tokenGenerator.GenerateToken(user.Id, user.Email, roles);

        user.RefreshToken = tokenResponse.RefreshToken;
        user.RefreshTokenExpiryTime = tokenResponse.RefreshTokenExpiresAt;
        
        await _userManager.UpdateAsync(user);
        
        return tokenResponse;
    }
}

