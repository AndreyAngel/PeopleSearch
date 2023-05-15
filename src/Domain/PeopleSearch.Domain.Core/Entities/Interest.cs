namespace PeopleSearch.Domain.Core.Entities;

public class Interest : BaseEntity
{
    // TODO: картинка интереса

    public string Name { get; set; }

    public string? Description { get; set; }
}
