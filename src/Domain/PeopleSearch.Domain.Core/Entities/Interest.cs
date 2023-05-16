namespace PeopleSearch.Domain.Core.Entities;

public class Interest
{
    // TODO: картинка интереса

    /// <summary>
    /// Gets or sets Id
    /// </summary>
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }
}
