namespace PeopleSearch.Domain.Core.Entities;

/// <summary>
/// Entity storing address data
/// </summary>
public class Address : BaseEntity
{
    /// <summary>
    /// Gests or sets a region
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    /// Gets or set a city
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets a street
    /// </summary>
    public string? Street { get; set; }

    /// <summary>
    /// Gets or sets a number of home
    /// </summary>
    public string? NumberOfHome { get; set; }
}
