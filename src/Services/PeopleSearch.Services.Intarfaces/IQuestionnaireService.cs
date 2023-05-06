using PeopleSearch.Domain.Core.Entities;

namespace PeopleSearch.Services.Interfaces;

public interface IQuestionnaireService : IDisposable
{
    List<UserQuestionnaire> GetAll();

    List<UserQuestionnaire> GetRecommendations(Guid userId);

    UserQuestionnaire GetById(Guid id);

    Task<UserQuestionnaire> Create(UserQuestionnaire userQuestionnaire);

    Task<UserQuestionnaire> Update(UserQuestionnaire userQuestionnaire);

    Task<UserQuestionnaire> ResetStatistics(Guid userId);
}
