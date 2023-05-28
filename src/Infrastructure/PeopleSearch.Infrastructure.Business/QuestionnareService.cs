using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Infrastructure.Exceptions;
using PeopleSearch.Domain.Core.Entities;
using PeopleSearch.Domain.Core.Enums;
using PeopleSearch.Domain.Interfaces;
using PeopleSearch.Infrastructure.RecommenderSystem;
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

    private readonly Dictionary<Guid, int> _userNumbers = new();

    private readonly Dictionary<int, Guid> _questionnaireNumbers = new();

    public QuestionnareService(IUnitOfWork db)
    {
        _db = db;
        _mapper = new Mapper(CreateMapperConfiguration());
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

        var entities = new List<UserQuestionnaire>();
        var matrixAllGrades = GetMatrixAllGrades();

        if (matrixAllGrades.Count != 0)
        {
            SVD.Initialize(matrixAllGrades, 3);
            var predictions = SVD.Predict().Where(x => x.UserNumber == _userNumbers[userId]).ToList();
            predictions = new List<Prediction>(predictions.OrderBy(x => x.PredictedGrade));

            if (predictions.Count == 0)
            {
                entities = _db.Questionnaires.Include(x => x.Address, x => x.Interests);
            }
            else
            {
                for (int i = 0; i < predictions.Last().UserNumber; i++)
                {
                    entities.Add(_db.Questionnaires.Include(x => x.Address, x => x.Interests)
                                    .Single(x => x.Id == _questionnaireNumbers[predictions[i].ItemNumber]));
                }
            }
        }

        entities = _db.Questionnaires.Include(x => x.Address, x => x.Interests);
        var recommendatedQuestionnaires = _mapper.Map<List<UserQuestionnaireModel>>(entities);

        return recommendatedQuestionnaires;
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

    public async Task Create(UserQuestionnaireModel model)
    {
        ThrowIfDisposed();

        var questionnaire = _db.Questionnaires.GetById(model.Id);

        if (questionnaire != null)
        {
            throw new ObjectNotUniqueException("This user already has a questionnaire", nameof(questionnaire));
        }

        var userQuestionnaire = _mapper.Map<UserQuestionnaire>(model);
        await _db.Questionnaires.AddAsync(userQuestionnaire);
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

    public async Task<UserQuestionnaireModel> Update(UserQuestionnaireUpdateModel model)
    {
        ThrowIfDisposed();

        var questionnaire = _db.Questionnaires.GetById(model.Id);

        if (questionnaire == null)
        {
            throw new NotFoundException("Questionnare with this Id wasn't founded", nameof(model.Id));
        }

        questionnaire.Name = model.Name;
        questionnaire.Surname = model.Surname;
        questionnaire.BirthDate = model.BirthDate;
        questionnaire.Address = _mapper.Map<Address>(model.Address);
        questionnaire.Interests = _mapper.Map<List<Interest>>(model.Interests);

        return _mapper.Map<UserQuestionnaireModel>(questionnaire);
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

    /// <summary>
    /// Create mapper configuration
    /// </summary>
    /// <returns><see cref="MapperConfiguration"/></returns>
    private static MapperConfiguration CreateMapperConfiguration()
    {
        return new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserQuestionnaire, UserQuestionnaireModel>();
            cfg.CreateMap<UserQuestionnaireModel, UserQuestionnaire>();

            cfg.CreateMap<AddressModel, Address>();
            cfg.CreateMap<Address, AddressModel>();

            cfg.CreateMap<UserModel, User>();
            cfg.CreateMap<User, UserModel>();

            cfg.CreateMap<GradeModel, Grade>();
            cfg.CreateMap<Grade, GradeModel>();

            cfg.CreateMap<InterestModel, Interest>();
            cfg.CreateMap<Interest, InterestModel>();
        });
    }

    private List<List<double>> GetMatrixAllGrades()
    {
        var grades = _db.Grades.GetAll();
        int countUsers = (int)Math.Sqrt(grades.Count);

        List<List<double>> matrixsGrades = new();

        for (int i = 0; i < countUsers; i++)
        {
            _userNumbers.Add(grades[i].UserId, i);
            matrixsGrades.Add(new List<double>());

            for (int j = 0; j < countUsers; j++)
            {
                matrixsGrades[i].Add((int)grades[i * countUsers + j].GradeValue);

                if (i == 0)
                {
                    _questionnaireNumbers.Add(j, grades[i * countUsers + j].QuestionnaireId);
                }
            }
        }

        return matrixsGrades;
    }
}
