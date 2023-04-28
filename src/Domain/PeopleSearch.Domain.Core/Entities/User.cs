using Microsoft.AspNetCore.Identity;

namespace PeopleSearch.Domain.Core.Entities;

/// <summary>
/// Entity storing user data
/// </summary>
public class User : IdentityUser
{
    /// <summary>
    /// Get or set user questionnaire Id
    /// </summary>
    public Guid? UserQuestionnaireId { get; set; }

    /// <summary>
    /// Get or set user questionnaire
    /// </summary>
    public UserQuestionnaire? UserQuestionnaire { get; set; }

    /// <summary>
    /// Gets a registration date
    /// </summary>
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
}
