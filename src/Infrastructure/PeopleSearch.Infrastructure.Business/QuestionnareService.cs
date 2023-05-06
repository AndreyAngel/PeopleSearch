using Infrastructure.Exceptions;
using PeopleSearch.Domain.Core.Entities;
using PeopleSearch.Domain.Interfaces;
using PeopleSearch.Services.Interfaces;
using PeopleSearch.Services.Interfaces.Exceptions;

namespace PeopleSearch.Infrastructure.Business;

public class QuestionnareService : IQuestionnaireService
{
    private readonly IUnitOfWork _db;

    private bool _isDisposed;

    public QuestionnareService(IUnitOfWork db)
    {
        _db = db;
    }

    public List<UserQuestionnaire> GetAll()
    {
        return _db.Questionnaires.GetAll();
    }

    public List<UserQuestionnaire> GetRecommendations(Guid userId)
    {
        throw new NotImplementedException();
    }

    public UserQuestionnaire GetById(Guid id)
    {
        var res = _db.Questionnaires.GetById(id);

        if (res == null)
        {
            throw new NotFoundException("Questionnare with this Id wasn't founded", nameof(id));
        }

        return res;
    }

    public async Task<UserQuestionnaire> Create(UserQuestionnaire userQuestionnaire)
    {
        var questionnaire = _db.Questionnaires.GetById(userQuestionnaire.Id);

        if (questionnaire != null)
        {
            throw new ObjectNotUniqueException("Questionnaire with this Id already exists", nameof(questionnaire));
        }

        await _db.Questionnaires.AddAsync(userQuestionnaire);
        return userQuestionnaire;
    }

    public async Task<UserQuestionnaire> Update(UserQuestionnaire userQuestionnaire)
    {
        var questionnaire = _db.Questionnaires.GetById(userQuestionnaire.Id);

        if (questionnaire == null)
        {
            throw new NotFoundException("Questionnare with this Id wasn't founded", nameof(userQuestionnaire.Id));
        }

        await _db.Questionnaires.UpdateAsync(userQuestionnaire);

        return userQuestionnaire;
    }

    public async Task<UserQuestionnaire> ResetStatistics(Guid userId)
    {
        var questionnaire = _db.Questionnaires.GetById(userId);

        if (questionnaire == null)
        {
            throw new NotFoundException("Questionnare with this Id wasn't founded", nameof(userId));
        }

        questionnaire.Likes = 0;
        questionnaire.Dislikes = 0;
        questionnaire.Views = 0;

        await _db.Questionnaires.UpdateAsync(questionnaire);

        return questionnaire;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // TODO: освободить управляемое состояние (управляемые объекты)
            }

            // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
            // TODO: установить значение NULL для больших полей
            _isDisposed = true;
        }
    }

    // // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
    // ~QuestionnareService()
    // {
    //     // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
