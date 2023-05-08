using PeopleSearch.Services.Intarfaces.Models;

namespace PeopleSearch.Services.Interfaces;

public interface IQuestionnaireService : IDisposable
{
    List<UserQuestionnaireModel> GetAll();

    List<UserQuestionnaireModel> GetRecommendations(Guid userId);

    UserQuestionnaireModel GetById(Guid id);

    Task<UserQuestionnaireModel> Create(UserQuestionnaireModel userQuestionnaire);

    Task<UserQuestionnaireModel> Update(UserQuestionnaireModel userQuestionnaire);

    Task<UserQuestionnaireModel> ResetStatistics(Guid userId);

    Task Publish(Guid userId);

    Task RemoveFromPublication(Guid userId);
}
