using PeopleSearch.Services.Intarfaces.Models;

namespace PeopleSearch.Services.Interfaces;

public interface IQuestionnaireService : IDisposable
{
    List<UserQuestionnaireModel> GetAll();

    List<UserQuestionnaireModel> GetRecommendations(Guid userId);

    UserQuestionnaireModel GetById(Guid id, Guid viewerId);

    Task Create(UserQuestionnaireModel userQuestionnaire);

    Task PutAGrade(GradeModel model);

    Task<UserQuestionnaireModel> Update(UserQuestionnaireUpdateModel userQuestionnaire);

    Task<UserQuestionnaireModel> ResetStatistics(Guid userId);

    Task Publish(Guid userId);

    Task RemoveFromPublication(Guid userId);
}
