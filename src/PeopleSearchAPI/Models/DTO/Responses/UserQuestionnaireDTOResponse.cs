namespace PeopleSearchAPI.Models.DTO.Responses;

public class UserQuestionnaireDTOResponse
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Surname
    /// </summary>
    public string? Surname { get; set; }

    /// <summary>
    /// Birth date
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// Address
    /// </summary>
    public AddressDTO? Address { get; set; }

    public List<InterestDTO> Interests { get; set; } = new();

    /// <summary>
    /// User Id
    /// </summary>
    public Guid UserId { get; set; }

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
