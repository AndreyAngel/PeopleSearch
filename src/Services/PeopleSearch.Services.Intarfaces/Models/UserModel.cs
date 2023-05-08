namespace PeopleSearch.Services.Intarfaces.Models;

/// <summary>
/// The data transfer object of the response containing the user data
/// </summary>
public class UserModel : IBaseModel
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

    /// <summary>
    /// Constructs a new instance of <see cref="UserModel"/>
    /// </summary>
    public UserModel()
    {
        Id = Guid.NewGuid().ToString();
    }
}
