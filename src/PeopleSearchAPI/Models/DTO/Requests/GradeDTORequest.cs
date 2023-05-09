namespace PeopleSearchAPI.Models.DTO.Requests;

public class GradeDTORequest
{
    public Guid QuestionnaireId { get; set; }

    public GradeEnumDTO GradeValue { get; set; }
}
