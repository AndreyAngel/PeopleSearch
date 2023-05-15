namespace PeopleSearchAPI.Models.DTO.Response;

/// <summary>
/// The data transfer object of the response containing the user data
/// </summary>
public class UserDTOResponse : IDTOResponse
{
    /// <summary>
    /// User Id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    public string? PhoneNumber { get; set; }
}
