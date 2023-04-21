using System.ComponentModel.DataAnnotations;

namespace PeopleSearchAPI.Models.DTO.Requests;

/// <summary>
/// Model of request for get access token
/// </summary>
public class GetAccessTokenDTORequest
{
    /// <summary>
    /// Refresh token
    /// </summary>
    [Required]
    public string? RefreshToken { get; set; }
}
