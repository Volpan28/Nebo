namespace AstroMonitor.Domain.Identity;

public static class UserRoles
{
    public const string Admin = "Admin";
    public const string User = "User";
    
    public static List<string> GetAvailableRoles()
    {
        return new List<string> {Admin, User};
    }
}