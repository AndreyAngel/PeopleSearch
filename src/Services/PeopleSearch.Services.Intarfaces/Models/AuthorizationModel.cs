﻿namespace PeopleSearch.Services.Intarfaces.Models;

/// <summary>
/// The data transfer object of the response containing the access token and refresh token
/// </summary>
public class AuthorizationModel : IBaseModel
{
    /// <summary>
    /// User Id
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Access token lifetime in seconds
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Access token type
    /// </summary>
    public string TokenType { get; set; }

    /// <summary>
    /// Access token
    /// </summary>
    public string AccessToken { get; set; }

    /// <summary>
    /// Refresh token
    /// </summary>
    public string RefreshToken { get; set; }

    /// <summary>
    /// StreamChat token
    /// </summary>
    public string StreamChatToken { get; set; }

    /// <summary>
    /// Creates an instance of the <see cref="AuthorizationModel"/>.
    /// </summary>
    /// <param name="expiresIn"> Access token lifetime in seconds </param>
    /// <param name="accessToken"> Access token </param>
    /// <param name="refreshToken"> Refresh token </param>
    /// <param name="streamChatToken"> StreamChat token </param>
    /// <param name="tokenType"> Access token type </param>
    /// <param name="userId"> userId </param>
    public AuthorizationModel(int expiresIn,
                              string accessToken,
                              string refreshToken,
                              string streamChatToken,
                              string tokenType,
                              Guid userId)
    {
        ExpiresIn = expiresIn;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        StreamChatToken = streamChatToken;
        TokenType = tokenType;
        UserId = userId;
    }
}
