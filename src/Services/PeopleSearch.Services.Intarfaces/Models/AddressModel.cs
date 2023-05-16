namespace PeopleSearch.Services.Intarfaces.Models;

/// <summary>
/// Address data transfer object
/// </summary>
public class AddressModel
{
    public required string Region { get; set; }

    /// <summary>
    /// City
    /// </summary>
    public required string City { get; set; }

    /// <summary>
    /// Street
    /// </summary>
    public string? Street { get; set; }

    /// <summary>
    /// Number of home
    /// </summary>
    public string? NumberOfHome { get; set; }

    /// <summary>
    /// Apartment number
    /// </summary>
    public string? ApartmentNumber { get; set; }

    /// <summary>
    /// Postal code
    /// </summary>
    public string? PostalCode { get; set; }
}
