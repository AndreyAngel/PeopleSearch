namespace PeopleSearch.Services.Intarfaces.Models;

public class UserQuestionnaireUpdateModel
{
    /// <summary>
    /// Gets or sets Id
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
    /// Gets or sets a address
    /// </summary>
    public AddressModel? Address { get; set; }

    /// <summary>
    /// Gets or sets interests
    /// </summary>
    public List<InterestModel> Interests { get; set; }
}
