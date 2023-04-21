using PeopleSearch.Domain.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace PeopleSearch.Domain.Interfaces;

/// <summary>
/// Provides an abstraction for a store which manages user accounts
/// </summary>
public interface ICustomUserStore : IUserStore<User>
{
    /// <summary>
    /// Adds tokens <see cref="IEnumerable{Token}"/> in database context
    /// </summary>
    /// <param name="tokens"> Tokens </param>
    /// <returns> Task object </returns>
    public Task AddRangeTokenAsync(IEnumerable<Token> tokens);

    /// <summary>
    /// Blocks tokens: Field "IsActive" = false
    /// </summary>
    /// <param name="userId"> User Id </param>
    /// <returns> Task object containing of list of blocked tokens </returns>
    public Task<List<Token>> BlockTokens(Guid userId);

    /// <summary>
    /// Gets access token and refresh token by user Id
    /// </summary>
    /// <param name="userId"> User Id </param>
    /// <returns> Task object </returns>
    public Task<List<Token>> GetTokensByUserId(Guid userId);

    /// <summary>
    /// Gets token by value
    /// </summary>
    /// <param name="value"> Token value </param>
    /// <returns> Task object containing of <see cref="Token"/> </returns>
    Task<Token> GetToken(string value);
}
