namespace PeopleSearchAPI.Models.DTO.Requests;

public class UserQuestionnaireDTORequest
{
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
}
