using AutoMapper;
using Infrastructure.Exceptions;
using PeopleSearch.Domain.Core.Entities;
using PeopleSearch.Domain.Core.Enums;
using PeopleSearch.Domain.Interfaces;
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

            cfg.CreateMap<GradeModel, Grade>();
            cfg.CreateMap<Grade, GradeModel>();
        });

        _mapper = new Mapper(config);
    }

    public List<UserQuestionnaireModel> GetAll()
    {
        ThrowIfDisposed();
        var res = _db.Questionnaires.GetAll();
        return _mapper.Map<List<UserQuestionnaireModel>>(res);
    }

    public List<UserQuestionnaireModel> GetRecommendations(Guid userId)
    {
        ThrowIfDisposed();
        throw new NotImplementedException();
    }

    public UserQuestionnaireModel GetById(Guid id, Guid viewerId)
    {
        ThrowIfDisposed();

        var res = _db.Questionnaires.GetById(id);

        if (res == null)
        {
            throw new NotFoundException("Questionnare with this Id wasn't founded", nameof(id));
        }

        // TODO: реализовать счётчик просмотров

        return _mapper.Map<UserQuestionnaireModel>(res);
    }

    public async Task<UserQuestionnaireModel> Create(UserQuestionnaireModel model)
    {
        ThrowIfDisposed();

        var questionnaire = _db.Questionnaires.GetById(model.Id);

        if (questionnaire != null)
        {
            throw new ObjectNotUniqueException("This user already has a questionnaire", nameof(questionnaire));
        }

        var userQuestionnaire = _mapper.Map<UserQuestionnaire>(model);
        await _db.Questionnaires.AddAsync(userQuestionnaire);

        return model;
    }

    public async Task PutAGrade(GradeModel model)
    {
        var res = _db.Grades.GetAll().SingleOrDefault(x => x.UserId == model.UserId &&
                                                             x.QuestionnaireId == model.QuestionnaireId);

        if (res != null)
        {
            throw new ObjectNotUniqueException("The user has already rated this questionnaire", null);
        }

        var questionnaire = _db.Questionnaires.GetById(model.QuestionnaireId);

        if (questionnaire == null)
        {
            throw new NotFoundException("The questionnaire with this Id wasn't founded", nameof(model.QuestionnaireId));
        }

        var grade = _mapper.Map<Grade>(model);
        await _db.Grades.AddAsync(grade);
        
        if (model.GradeValue == GradeEnum.Like)
        {
            questionnaire.Likes += 1;
        }
        else
        {
            questionnaire.Dislikes += 1;
        }
    }

    public async Task<UserQuestionnaireModel> Update(UserQuestionnaireModel model)
    {
        ThrowIfDisposed();

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
        ThrowIfDisposed();

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
        ThrowIfDisposed();

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
        ThrowIfDisposed();

        var questionnaire = _db.Questionnaires.GetById(userId);

        if (questionnaire == null)
        {
            throw new NotFoundException("Questionnare with this Id wasn't founded", nameof(userId));
        }

        questionnaire.IsPublished = false;

        await _db.Questionnaires.UpdateAsync(questionnaire);
    }

    public void Dispose()
    {
        // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    protected void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _db.Dispose();
            }

            _isDisposed = true;
        }
    }

    /// <summary>
    /// Throws if this class has been disposed.
    /// </summary>
    protected void ThrowIfDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }
}
