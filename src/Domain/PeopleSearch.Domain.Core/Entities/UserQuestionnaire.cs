namespace PeopleSearch.Domain.Core.Entities;

/// <summary>
/// User questionnaire
/// </summary>
public class UserQuestionnaire
{
    /// <summary>
    /// Get or set a Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets a name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets a surname
    /// </summary>
    public string? Surname { get; set; }

    /// <summary>
    /// Gets or sets a birth date
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// Gets or sets a address Id
    /// </summary>
    public Guid? AddressId { get; set; }

    /// <summary>
    /// Gets or sets a address
    /// </summary>
    public Address? Address { get; set; }
}
