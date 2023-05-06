using PeopleSearch.Domain.Core.Enums;

namespace PeopleSearch.Domain.Core.Entities;

/// <summary>
/// Grade
/// </summary>
public class Grade
{
    /// <summary>
    /// Gets or sets Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or seys user Id
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Gets or sets questionnaire Id
    /// </summary>
    public Guid QuestionnaireId { get; set; }

    /// <summary>
    /// Gets or sets grade value
    /// </summary>
    public GradeEnum GradeValue { get; set; } = GradeEnum.None;
}
