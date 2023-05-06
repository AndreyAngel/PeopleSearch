using PeopleSearch.Domain.Core.Entities;
using PeopleSearch.Domain.Interfaces;

namespace PeopleSearch.Infrastructure.Data;
public class QuestionnaireRepository : GenericRepository<UserQuestionnaire>, IQuestionnaireRepository
{
    public QuestionnaireRepository(Context context) : base(context)
    {

    }
}
