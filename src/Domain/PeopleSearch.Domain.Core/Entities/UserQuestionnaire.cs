using System.ComponentModel.DataAnnotations.Schema;

namespace PeopleSearch.Domain.Core.Entities;

/// <summary>
/// User questionnaire
/// </summary>
public class UserQuestionnaire
{
    /// <summary>
    /// Get or set a Id
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    // TODO: добавить фото профиля

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

    public List<Interest> Interests { get; set; } = new();

    /// <summary>
    /// Gets or sets a user Id
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets a <see cref="Entities.User"/>
    /// </summary>
    public User? User { get; set; }

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

    public bool IsPublished { get; set; } = false;
}
