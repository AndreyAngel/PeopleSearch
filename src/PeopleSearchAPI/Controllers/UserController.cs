﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PeopleSearch.Domain.Core.Enums;
using PeopleSearch.Domain.Interfaces;
using PeopleSearch.Services.Intarfaces.Models;
using PeopleSearch.Services.Interfaces;
using PeopleSearch.Services.Interfaces.Exceptions;
using PeopleSearchAPI.Helpers;
using PeopleSearchAPI.Models.DTO.Requests;
using PeopleSearchAPI.Models.DTO.Response;
using System.Net;
using System.Security;

namespace PeopleSearchAPI.Controllers;

/// <summary>
/// Provides the APIs for handling all the user logic
/// </summary>
[ApiController]
[Route("api/v1/[controller]/[action]")]
public class UserController : ControllerBase
{
    /// <summary>
    /// Object of class <see cref="IUserService"/> providing the APIs for managing user in a persistence store.
    /// </summary>
    private readonly IUserService _userService;

    private readonly IQuestionnaireService _questionnaireService;

    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Object of class <see cref="IMapper"/> for models mapping
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    /// Creates an instance of the <see cref="UserController"/>.
    /// </summary>
    /// <param name="userService"> Object of class providing the APIs for managing user in a persistence store. </param>
    /// <param name="mapper"> Object of class <see cref="IMapper"/> for models mapping </param>
    public UserController(IUserService userService,
                          IQuestionnaireService questionnaireService,
                          IUnitOfWork unitOfWork,
                          IMapper mapper)
    {
        _userService = userService;
        _questionnaireService = questionnaireService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Get the user information by Id
    /// </summary>
    /// <param name="userId"> User Id </param>
    /// <returns> The task object containing the action result of getting user information </returns>
    /// <response code="200"> Successful completion </response>
    /// <response code="404"> User with this Id wasn't founded </response>
    /// <response code="401"> Unauthorized </response>
    [HttpGet("{userId:Guid}")]
    [CustomAuthorize]
    [ProducesResponseType(typeof(UserDTOResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetById(Guid userId)
    {
        try
        {
            var user = await _userService.GetById(userId);
            var response = _mapper.Map<UserDTOResponse>(user);

            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Get new access token with refresh token
    /// </summary>
    /// <param name="model"> Model of request for get access token </param>
    /// <returns> The task object containing the action result of get access token </returns>
    /// <response code="200"> Successful completion </response>
    /// <response code="403"> Insecure request </response>
    [HttpPost]
    [ProducesResponseType(typeof(AuthorizationDTOResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Forbidden)]
    public async Task<IActionResult> GetAccessTokens(GetAccessTokenDTORequest model)
    {
        try
        {
            var response = await _userService.GetAccessTokens(model.RefreshToken);
            return Ok(response);
        }
        catch (SecurityException ex)
        {
            return Forbid(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    /// <summary>
    /// Registration of the new user
    /// </summary>
    /// <param name="model"> Registration data transfer object </param>
    /// <returns> The task object containing the authorization result </returns>
    /// <response code="201"> User registrated </response>
    /// <response code="400"> Incorrect data was sent during registration </response>
    [Login]
    [HttpPost]
    [ProducesResponseType(typeof(AuthorizationDTOResponse), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(IdentityErrorsModel), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Registration(RegisterDTORequest model)
    {
        var user = _mapper.Map<UserModel>(model);
        var result = await _userService.Register(user, model.Password, Role.User);
        await _questionnaireService.Create(new UserQuestionnaireModel()
        {
            Id = new Guid(user.Id),
            UserId = new Guid(user.Id)
        });

        if (result.GetType() == typeof(IdentityErrorsModel))
        {
            return BadRequest(result);
        }

        //TODO: Решить задачу сохранения всех изменеий после завершения всех процессов
        //await _userService.Store.Context.SaveChangesAsync();
        await _unitOfWork.SaveChangesAsync();

        return Created(new Uri($"https://localhost:44389/api/v1/IdentityAPI/User/GetById/{user.Id}"), result);
    }

    /// <summary>
    /// Authorization of the user
    /// </summary>
    /// <param name="dto"> Login data transfer object </param>
    /// <returns> The task object containing the authorization result </returns>
    /// <response code="200"> Successful completion </response>
    /// <response code="404"> Incorrect data was sent during authorization </response>
    /// <response code="401"> Incorrect password </response>
    /// <response code="403"> Already authorized </response>
    [Login]
    [HttpPost]
    [ProducesResponseType(typeof(AuthorizationDTOResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(JsonResult), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> Login(LoginDTORequest dto)
    {
        try
        {
            var model = _mapper.Map<LoginModel>(dto);
            var response = await _userService.Login(model);
            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (IncorrectPasswordException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    /// <summary>
    /// Logout from account
    /// </summary>
    /// <returns> The task object </returns>
    /// <response code="204"> Successful completion </response>
    /// <response code="401"> Unauthorized </response>
    [HttpPost]
    [CustomAuthorize]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> Logout()
    {
        var user = HttpContext.Items["User"] as UserModel;
        await _userService.Logout(new Guid(user.Id));
        return NoContent();
    }

    ///<summary>
    /// Change password
    /// </summary>
    /// <param name="model"> Stores data for changing password </param>
    /// <returns> Task object contaning action result of changing password </returns>
    /// <response code="204"> Successful completion </response>
    /// <response code="400"> Bad request </response>
    /// <response code="401"> Unauthorized </response>
    [HttpPatch]
    [CustomAuthorize]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(IdentityErrorsModel), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ChangePassword(ChangePasswordDTORequest model)
    {
        try
        {
            var result = await _userService.ChangePassword(model.Email, model.OldPassword, model.NewPassword);

            if (result != null)
            {
                return BadRequest(result);
            }

            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
