using AutoMapper;
using Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PeopleSearch.Domain.Core.Entities;
using PeopleSearch.Domain.Interfaces;
using PeopleSearch.Services.Intarfaces.Models;
using PeopleSearch.Services.Intarfaces.Models;
using PeopleSearch.Services.Interfaces;
using PeopleSearch.Services.Interfaces.Exceptions;

namespace PeopleSearch.Infrastructure.Business;

public class QuestionnareService : IQuestionnaireService
{
    private readonly IUnitOfWork _db;

    /// <summary>
    /// Object of class <see cref="IMapper"/> for models mapping
    /// </summary>
    private readonly IMapper _mapper;

    private bool _isDisposed;

    public QuestionnareService(IUnitOfWork db)
    {
        _db = db;

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserQuestionnaire, UserQuestionnaireModel>();
            cfg.CreateMap<UserQuestionnaireModel, UserQuestionnaire>();

            cfg.CreateMap<AddressModel, Address>();
            cfg.CreateMap<Address, AddressModel>();

            cfg.CreateMap<UserModel, User>();
            cfg.CreateMap<User, UserModel>();
        });

        _mapper = new Mapper(config);
    }

    public List<UserQuestionnaireModel> GetAll()
    {
        var res = _db.Questionnaires.GetAll();
        return _mapper.Map<List<UserQuestionnaireModel>>(res);
    }

    public List<UserQuestionnaireModel> GetRecommendations(Guid userId)
    {
        throw new NotImplementedException();
    }

    public UserQuestionnaireModel GetById(Guid id)
    {
        var res = _db.Questionnaires.GetById(id);

        if (res == null)
        {
            throw new NotFoundException("Questionnare with this Id wasn't founded", nameof(id));
        }

        return _mapper.Map<UserQuestionnaireModel>(res);
    }

    public async Task<UserQuestionnaireModel> Create(UserQuestionnaireModel model)
    {
        var questionnaire = _db.Questionnaires.GetById(model.Id);

        if (questionnaire != null)
        {
            throw new ObjectNotUniqueException("Questionnaire with this Id already exists", nameof(questionnaire));
        }

        var userQuestionnaire = _mapper.Map<UserQuestionnaire>(model);
        await _db.Questionnaires.AddAsync(userQuestionnaire);

        return model;
    }

    public async Task<UserQuestionnaireModel> Update(UserQuestionnaireModel model)
    {
        var questionnaire = _db.Questionnaires.GetById(model.Id);

        if (questionnaire == null)
        {
            throw new NotFoundException("Questionnare with this Id wasn't founded", nameof(model.Id));
        }

        var userQuestionnaire = _mapper.Map<UserQuestionnaire>(model);
        await _db.Questionnaires.UpdateAsync(userQuestionnaire);

        return model;
    }

    public async Task<UserQuestionnaireModel> ResetStatistics(Guid userId)
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

        return _mapper.Map<UserQuestionnaireModel>(questionnaire);
    }

    public async Task Publish(Guid userId)
    {
        var questionnaire = _db.Questionnaires.GetById(userId);

        if (questionnaire == null)
        {
            throw new NotFoundException("Questionnare with this Id wasn't founded", nameof(userId));
        }

        questionnaire.IsPublished = true;

        await _db.Questionnaires.UpdateAsync(questionnaire);
    }

    public async Task RemoveFromPublication(Guid userId)
    {
        var questionnaire = _db.Questionnaires.GetById(userId);

        if (questionnaire == null)
        {
            throw new NotFoundException("Questionnare with this Id wasn't founded", nameof(userId));
        }

        questionnaire.IsPublished = false;

        await _db.Questionnaires.UpdateAsync(questionnaire);
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
