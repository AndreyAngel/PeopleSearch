namespace PeopleSearch.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    public IGradeRepository Grades { get; }

    public IQuestionnaireRepository Questionnaires { get; }

    public Task SaveChangesAsync();
}
