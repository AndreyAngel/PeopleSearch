using AutoMapper;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using PeopleSearch.Domain.Interfaces;
using PeopleSearch.Services.Intarfaces.Models;
using PeopleSearch.Services.Interfaces;
using PeopleSearch.Services.Interfaces.Exceptions;
using PeopleSearchAPI.Helpers;
using PeopleSearchAPI.Models.DTO.Requests;
using PeopleSearchAPI.Models.DTO.Responses;
using System.Net;

namespace PeopleSearchAPI.Controllers;

/// <summary>
/// Provides the APIs for handling all the questionnaire logic
/// </summary>
[Route("api/v1/[controller]/[action]")]
[ApiController]
[CustomAuthorize]
public class QuestionnaireController : ControllerBase
{
    private readonly IQuestionnaireService _questionnaireService;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    /// <summary>
    /// Creates an instance of the <see cref="QuestionnaireController"/>.
    /// </summary>
    /// <param name="questionnaireService"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="mapper"></param>
    public QuestionnaireController(IQuestionnaireService questionnaireService,
                                    IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _questionnaireService = questionnaireService;
    }

    /// <summary>
    /// Get recommendations
    /// </summary>
    /// <returns> The action result of getting recommendations </returns>
    /// <response code="200"> Successful completion </response>
    [HttpGet]
    [ProducesResponseType(typeof(UserQuestionnaireListDTOResponse), (int)HttpStatusCode.OK)]
    public IActionResult GetRecommendations()
    {
        var user = HttpContext.Items["User"] as UserModel;
        var result = _questionnaireService.GetRecommendations(new Guid(user.Id));
        var response = _mapper.Map<List<UserQuestionnaireListDTOResponse>>(result);

        return Ok(response);
    }

    /// <summary>
    /// Get questionnare by user Id
    /// </summary>
    /// <param name="userId"> User Id </param>
    /// <returns> The action result of getting questionnaire </returns>
    /// <response code="200"> Successful completion </response>
    /// <response code="404"> The user questionnare wasn't founded </response>
    [HttpGet("{userId:Guid}")]
    [ProducesResponseType(typeof(UserQuestionnaireDTOResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    public IActionResult GetByUserId(Guid userId)
    {
        try
        {
            var viewer = HttpContext.Items["User"] as UserModel;
            var result = _questionnaireService.GetById(userId, new Guid(viewer.Id));
            var response = _mapper.Map<UserQuestionnaireDTOResponse>(result);

            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Fill out a form
    /// </summary>
    /// <param name="request"> User questionnaire </param>
    /// <returns> The task object contains the action result of creating user questionnaire </returns>
    /// <response code="200"> Successful completion </response>
    [HttpPatch]
    [ProducesResponseType(typeof(UserQuestionnaireDTOResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> FillOutAForm(UserQuestionnaireDTORequest request)
    {
        var model = _mapper.Map<UserQuestionnaireUpdateModel>(request);

        var user = HttpContext.Items["User"] as UserModel;
        model.Id = new Guid(user.Id);

        var result = await _questionnaireService.Update(model);
        await _questionnaireService.Publish(new Guid(user.Id));
        result.IsPublished = true;

        await _unitOfWork.SaveChangesAsync();
            
        var response = _mapper.Map<UserQuestionnaireDTOResponse>(result);

        return Ok(response);
    }

    /// <summary>
    /// Put a grade
    /// </summary>
    /// <param name="request"> Grade </param>
    /// <returns> The task contains the action result putting a grade </returns>
    /// <response code="204"> Successful completion </response>
    /// <response code="404"> The user questionnare wasn't founded </response>
    /// <response code="409"> Grade for this questionnare is already put </response>
    [HttpPatch]
    [ProducesResponseType(typeof(UserQuestionnaireDTOResponse), (int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
    public async Task<IActionResult> PutAGrade(GradeDTORequest request)
    {
        try
        {
            var viewer = HttpContext.Items["User"] as UserModel;

            var grade = _mapper.Map<GradeModel>(request);
            grade.UserId = new Guid(viewer.Id);

            await _questionnaireService.PutAGrade(grade);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ObjectNotUniqueException ex)
        {
            return Conflict(ex.Message);
        }
    }

    /// <summary>
    /// Update user questionnaire
    /// </summary>
    /// <param name="request"> Updated user questionnare </param>
    /// <returns> The task object contains the action result of updating user questionnaire </returns>
    /// <response code="200"> Successful completion </response>
    [HttpPatch]
    [ProducesResponseType(typeof(UserQuestionnaireDTOResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Update(UserQuestionnaireDTORequest request)
    {
        var model = _mapper.Map<UserQuestionnaireUpdateModel>(request);

        var user = HttpContext.Items["User"] as UserModel;
        model.Id = new Guid(user.Id);

        var result = await _questionnaireService.Update(model);
        await _unitOfWork.SaveChangesAsync();
            
        var response = _mapper.Map<UserQuestionnaireDTOResponse>(result);

        return Ok(response);
    }

    /// <summary>
    /// Resert statistics
    /// </summary>
    /// <returns> The task object contains the action result of reserting statisctics </returns>
    /// <response code="200"> Successful completion </response>
    [HttpPatch]
    [ProducesResponseType(typeof(UserQuestionnaireDTOResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ResetStatistics()
    {
        var user = HttpContext.Items["User"] as UserModel;
        var result = await _questionnaireService.ResetStatistics(new Guid(user.Id));
        await _unitOfWork.SaveChangesAsync();

        var response = _mapper.Map<UserQuestionnaireDTOResponse>(result);

        return Ok(response);
    }

    /// <summary>
    /// Publish your questionnaire
    /// </summary>
    /// <returns> The task object contains the action result of publishing your questionnare </returns>
    /// <response code="204"> Successful completion </response>
    [HttpPatch]
    [ProducesResponseType(typeof(UserQuestionnaireDTOResponse), (int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> Publish()
    {
        var user = HttpContext.Items["User"] as UserModel;
        await _questionnaireService.Publish(new Guid(user.Id));
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Remove from publication
    /// </summary>
    /// <returns> The task object contains the action result of removing from publication </returns>
    /// <response code="204"> Successful completion </response>
    [HttpPatch]
    [ProducesResponseType(typeof(UserQuestionnaireDTOResponse), (int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> RemoveFromPublication()
    {
        var user = HttpContext.Items["User"] as UserModel;
        await _questionnaireService.RemoveFromPublication(new Guid(user.Id));
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}
