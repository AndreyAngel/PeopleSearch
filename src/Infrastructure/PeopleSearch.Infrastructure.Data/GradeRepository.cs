using PeopleSearch.Domain.Core.Entities;
using PeopleSearch.Domain.Interfaces;

namespace PeopleSearch.Infrastructure.Data;

public class GradeRepository : GenericRepository<Grade>, IGradeRepository
{
    public GradeRepository(Context context) : base(context)
    {

    }
}
