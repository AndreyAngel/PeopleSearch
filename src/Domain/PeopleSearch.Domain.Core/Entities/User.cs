using Microsoft.AspNetCore.Identity;

namespace PeopleSearch.Domain.Core.Entities;

/// <summary>
/// Entity storing user data
/// </summary>
public class User : IdentityUser
{
    /// <summary>
    /// Gets a registration date
    /// </summary>
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
}
