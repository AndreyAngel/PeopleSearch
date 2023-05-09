using PeopleSearch.Domain.Core.Enums;

namespace PeopleSearch.Services.Intarfaces.Models;

public class GradeModel
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid QuestionnaireId { get; set; }

    public GradeEnum GradeValue { get; set; }
}
