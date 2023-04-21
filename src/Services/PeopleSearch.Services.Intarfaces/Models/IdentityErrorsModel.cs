using Microsoft.AspNetCore.Identity;

namespace PeopleSearch.Services.Intarfaces.DTO;

/// <summary>
/// The data transfer object of the response containing the identity errors
/// </summary>
public class IdentityErrorsModel : IBaseModel
{
    /// <summary>
    /// Identity errors list
    /// </summary>
    public IEnumerable<IdentityError> Errors { get; set; }

    /// <summary>
    /// Creates an instance of the <see cref="IdentityErrorsModel"/>.
    /// </summary>
    /// <param name="errors"> Identity errors list </param>
    public IdentityErrorsModel(IEnumerable<IdentityError> errors)
    {
        Errors = errors;
    }
}
