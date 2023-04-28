using System.ComponentModel.DataAnnotations;

namespace PeopleSearchAPI.Models.DTO;

/// <summary>
/// Address data transfer object
/// </summary>
public class AddressDTO
{
    /// <summary>
    /// Region
    /// </summary>
    [Required]
    public string? Region { get; set; }

    /// <summary>
    /// City
    /// </summary>
    [Required]
    public string? City { get; set; }

    /// <summary>
    /// Street
    /// </summary>
    public string? Street { get; set; }

    /// <summary>
    /// Number of home
    /// </summary>
    public string? NumberOfHome { get; set; }
}
