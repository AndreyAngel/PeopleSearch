using PeopleSearch.Domain.Core.Entities;
using PeopleSearch.Services.Intarfaces.Models;

namespace PeopleSearch.Services.Intarfaces.Models;

public class UserQuestionnaireModel
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
    public AddressModel? Address { get; set; }

    public Guid UserId { get; set; }

    public UserModel? User { get; set; }

    /// <summary>
    /// Gest or sets count of likes
    /// </summary>
    public int Likes { get; set; } = 0;

    /// <summary>
    /// Gets or sets count of dislikes
    /// </summary>
    public int Dislikes { get; set; } = 0;

    /// <summary>
    /// Gets or sets views
    /// </summary>
    public int Views { get; set; } = 0;

    public bool IsPublished { get; set; } = true;
}
