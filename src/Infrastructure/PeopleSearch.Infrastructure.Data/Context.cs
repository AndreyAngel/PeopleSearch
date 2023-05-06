using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PeopleSearch.Domain.Core.Entities;

namespace PeopleSearch.Infrastructure.Data;

/// <summary>
/// Class for the Entity Framework database context used for identity
/// </summary>
public class Context : IdentityDbContext<User>
{
    /// <summary>
    /// Creates an instance of the <see cref="Context"/>.
    /// </summary>
    /// <param name="options"> <see cref="DbContextOptions{Context}"/> </param>
    public Context(DbContextOptions<Context> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<UserQuestionnaire> UserQuestionnaires { get; set; }

    public DbSet<Address> Addresses { get; set; }

    public DbSet<Token> Tokens { get; set; }

    public DbSet<Grade> Grades { get; set; }
}
