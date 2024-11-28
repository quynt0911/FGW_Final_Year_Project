using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
    public string ProfileImagePath { get; set; }
    public bool IsEmailConfirmed { get; set; }
}

