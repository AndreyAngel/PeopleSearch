using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PeopleSearch.Domain.Core.Entities;
using PeopleSearch.Domain.Core.Enums;
using PeopleSearch.Domain.Interfaces;
using PeopleSearch.Infrastructure.Business.Helpers;
using PeopleSearch.Services.Intarfaces.DTO;
using PeopleSearch.Services.Interfaces;
using PeopleSearch.Services.Interfaces.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;

namespace PeopleSearch.Infrastructure.Business;

/// <summary>
/// Provides the APIs for managing user in a persistence store.
/// </summary>
public class UserService : UserManager<User>, IUserService
{
    /// <summary>
    /// Configurations of application
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Object of class <see cref="IMapper"/> for models mapping
    /// </summary>
    private readonly IMapper _mapper;

    /// <inheritdoc/>
    private readonly RoleManager<IdentityRole> _roleManager;

    /// <summary>
    /// True, if object is disposed
    /// False, if object isn't disposed
    /// </summary>
    private bool _disposed = false;

    /// <inheritdoc/>
    public new ICustomUserStore Store { get; init;  }

    /// <summary>
    /// Constructs a new instance of <see cref="UserManager{TUser}"/>.
    /// </summary>
    /// <param name="configuration"> Configurations of application </param>
    /// <param name="store">The persistence store the manager will operate over.</param>
    /// <param name="optionsAccessor">The accessor used to access the <see cref="IdentityOptions"/>.</param>
    /// <param name="passwordHasher">The password hashing implementation to use when saving passwords.</param>
    /// <param name="userValidators">A collection of <see cref="IUserValidator{TUser}"/> to validate users against.</param>
    /// <param name="passwordValidators">A collection of <see cref="IPasswordValidator{TUser}"/> to validate passwords against.</param>
    /// <param name="keyNormalizer">The <see cref="ILookupNormalizer"/> to use when generating index keys for users.</param>
    /// <param name="errors">The <see cref="IdentityErrorDescriber"/> used to provider error messages.</param>
    /// <param name="services">The <see cref="IServiceProvider"/> used to resolve services.</param>
    /// <param name="roleManager"> Provides the APIs for managing roles in a persistence store. </param>
    /// <param name="logger">The logger used to log messages, warnings and errors.</param>
    public UserService( IConfiguration configuration,
                        ICustomUserStore store,
                        IOptions<IdentityOptions> optionsAccessor,
                        IPasswordHasher<User> passwordHasher,
                        IEnumerable<IUserValidator<User>> userValidators,
                        IEnumerable<IPasswordValidator<User>> passwordValidators,
                        ILookupNormalizer keyNormalizer,
                        IdentityErrorDescriber errors,
                        IServiceProvider services,
                        RoleManager<IdentityRole> roleManager,
                        ILogger<UserManager<User>> logger) : base(store,
                                                                  optionsAccessor,
                                                                  passwordHasher,
                                                                  userValidators,
                                                                  passwordValidators,
                                                                  keyNormalizer,
                                                                  errors,
                                                                  services,
                                                                  logger)
    {
        _configuration = configuration;
        Store = store;
        _roleManager = roleManager;

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<User, UserModel>();
            cfg.CreateMap<UserModel, User>()
                .ForMember(x => x.UserName, opt => opt.MapFrom(src => src.Email));
    });

        _mapper = new Mapper(config);
    }

    /// <inheritdoc/>
    public async Task<UserModel> GetById(Guid id)
    {
        ThrowIfDisposed();
        var user = await FindByIdAsync(id.ToString());

        if (user == null)
        {
            throw new NotFoundException("User with this Id wasn't founded", nameof(id));
        }

        return _mapper.Map<UserModel>(user);
    }

    /// <inheritdoc/>
    public async Task<AuthorizationModel> Login(LoginModel model)
    {
        ThrowIfDisposed();
        var user = await FindByEmailAsync(model.Email);

        if (user == null)
        {
            throw new NotFoundException("User with this Email wasn't founded", nameof(model.Email));
        }

        return await Login(user, model.Password);
    }

    /// <inheritdoc/>
    public async Task<IBaseModel> Register(UserModel userAppDTO, string password, Role role)
    {
        ThrowIfDisposed();

        var user = _mapper.Map<User>(userAppDTO);
        var userRole = new IdentityRole { Name = Enum.GetName(typeof(Role), role) };
        var result = await CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return new IdentityErrorsModel(result.Errors);
        }

        try
        {
            await AddToRoleAsync(user, userRole.Name);
        }
        catch (InvalidOperationException)
        {
            await _roleManager.CreateAsync(userRole);
            await AddToRoleAsync(user, userRole.Name);
        }

        var claims = new List<Claim>()
        {
            new Claim("UserId", user.Id),
            new Claim(ClaimTypes.Role, userRole.Name)
        };

        await AddClaimsAsync(user, claims);

        return await GenerateTokens(new Guid(user.Id), userRole.Name, claims);
    }

    /// <inheritdoc/>
    public async Task<AuthorizationModel> GetAccessToken(string refreshToken)
    {
        ThrowIfDisposed();
        var validatedToken = JwtTokenHelper.ValidateToken(_configuration, refreshToken);

        var userId = new JwtSecurityToken(validatedToken).Claims.ToList().FirstOrDefault(x => x.Type == "UserId");
        

        if (userId == null)
        {
            throw new SecurityException("Incorrect refreshToken");
        }

        if (!await TokenIsActive(refreshToken))
        {
            throw new UnauthorizedAccessException("Unauthorization");
        }

        var user = await FindByIdAsync(userId.Value);

        if (user == null)
        {
            throw new SecurityException("Incorrect refreshToken");
        }

        var roles = await GetRolesAsync(user);

        if (roles.Count == 0)
        {
            throw new SecurityException("Incorrect refreshToken");
        }

        return await GenerateTokens(new Guid(userId.Value), roles[0]);
    }

    /// <inheritdoc/>
    public async Task Logout(Guid userId)
    {
        ThrowIfDisposed();
        await Store.BlockTokens(userId);
    }

    /// <inheritdoc/>
    public async Task<bool> TokenIsActive(string token)
    {
        ThrowIfDisposed();

        return (await Store.GetToken(token)).IsActive;
    }

    /// <inheritdoc/>
    public async Task<IBaseModel?> Update(UserModel userAppDTO, Guid userId)
    {
        ThrowIfDisposed();

        var user = _mapper.Map<User>(userAppDTO);
        var res = await FindByIdAsync(userId.ToString());

        if (res == null)
        {
            throw new NotFoundException("User with this Id wasn't founded", nameof(userId));
        }

        res.UserQuestionnaire.Name = user.UserQuestionnaire.Name ?? res.UserQuestionnaire.Name;
        res.UserQuestionnaire.Surname = user.UserQuestionnaire.Surname ?? res.UserQuestionnaire.Surname;
        res.UserQuestionnaire.BirthDate = user.UserQuestionnaire.BirthDate ?? res.UserQuestionnaire.BirthDate;
        res.UserQuestionnaire.Address = user.UserQuestionnaire.Address ?? res.UserQuestionnaire.Address;

        var result = await UpdateAsync(res);

        if (!result.Succeeded)
        {
            return new IdentityErrorsModel(result.Errors);
        }

        var userDTO = _mapper.Map<UserModel>(res);

        return userDTO;
    }

    /// <inheritdoc/>
    public async Task<IBaseModel?> ChangePassword(string email, string oldPassword, string newPassword)
    {
        ThrowIfDisposed();
        var user = await FindByEmailAsync(email);

        if (user == null)
        {
            throw new NotFoundException("User with this Email wasn't founded", nameof(email));
        }

        var result = await ChangePasswordAsync(user, oldPassword, newPassword);

        if (!result.Succeeded)
        {
            return new IdentityErrorsModel(result.Errors);
        }

        return null;
    }

    /// <inheritdoc/>
    protected new void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
            }

            _disposed = true;
        }

        base.Dispose(disposing);
    }

    /// <inheritdoc/>
    private async Task<AuthorizationModel> Login(User user, string password)
    {
        ThrowIfDisposed();
        if (!await CheckPasswordAsync(user, password))
        {
            throw new IncorrectPasswordException("Incorrect password", nameof(password));
        }

        var roles = await GetRolesAsync(user);

        return await GenerateTokens(new Guid(user.Id), roles[0]);
    }

    /// <inheritdoc/>
    private async Task<AuthorizationModel> GenerateTokens(Guid userId, string roleName, List<Claim>? claims = null)
    {
        ThrowIfDisposed();
        if (claims == null)
        {
            claims = new List<Claim>()
            {
                new Claim("UserId", userId.ToString()),
                new Claim(ClaimTypes.Role, roleName)
            };
        }

        var refreshToken = JwtTokenHelper.GenerateJwtRefreshToken(_configuration, new List<Claim>() { claims[0] });
        var accessToken = JwtTokenHelper.GenerateJwtAccessToken(_configuration, claims);

        await Store.BlockTokens(userId);

        await Store.AddRangeTokenAsync(new List<Token>()
        {
            new Token()
            {
                UserId = userId,
                TokenType = TokenType.Access,
                Value = accessToken
            },

            new Token()
            {
                UserId = userId,
                TokenType = TokenType.Refresh,
                Value = refreshToken
            }
        });

        return new AuthorizationModel(900, accessToken, refreshToken, "Bearer", userId);
    }
}