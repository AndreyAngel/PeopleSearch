using System.ComponentModel.DataAnnotations;

namespace PeopleSearch.Services.Intarfaces.Models;

/// <summary>
/// Login data transfer object
/// </summary>
public class LoginModel
{
    /// <summary>
    /// Email
    /// </summary>
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    [Required]
    public string? Password { get; set; }
}
