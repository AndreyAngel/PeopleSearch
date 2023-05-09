using AutoMapper;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using PeopleSearch.Domain.Core.Enums;
using PeopleSearch.Domain.Interfaces;
using PeopleSearch.Services.Intarfaces.Models;
using PeopleSearch.Services.Interfaces;
using PeopleSearch.Services.Interfaces.Exceptions;
using PeopleSearchAPI.Models.DTO.Requests;
using PeopleSearchAPI.Models.DTO.Responses;

namespace PeopleSearchAPI.Controllers;

[Route("api/v1/[controller]/[action]")]
[ApiController]
public class QuestionnareController : ControllerBase
{
    private readonly IQuestionnaireService _questionnaireService;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    public QuestionnareController(IQuestionnaireService questionnaireService,
                                    IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _questionnaireService = questionnaireService;
    }

    [HttpGet]
    public IActionResult GetRecommendations()
    {
        var user = HttpContext.Items["User"] as UserModel;
        var result = _questionnaireService.GetRecommendations(new Guid(user.Id));
        var response = _mapper.Map<UserQuestionnaireListDTOResponse>(result);

        return Ok(response);
    }

    [HttpGet("{userId:Guid}")]
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

    [HttpPost]
    public async Task<IActionResult> Create(UserQuestionnaireDTORequest request)
    {
        var model = _mapper.Map<UserQuestionnaireModel>(request);

        var user = HttpContext.Items["User"] as UserModel;
        model.Id = new Guid(user.Id);
        model.UserId = new Guid(user.Id);

        var result = await _questionnaireService.Create(model);
        await _unitOfWork.SaveChangesAsync();
            
        var response = _mapper.Map<UserQuestionnaireDTOResponse>(result);

        return Ok(response);
    }

    [HttpPost]
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

    [HttpPatch]
    public async Task<IActionResult> Update(UserQuestionnaireDTORequest request)
    {
        var model = _mapper.Map<UserQuestionnaireModel>(request);

        var user = HttpContext.Items["User"] as UserModel;
        model.Id = new Guid(user.Id);

        var result = await _questionnaireService.Update(model);
        await _unitOfWork.SaveChangesAsync();
            
        var response = _mapper.Map<UserQuestionnaireDTOResponse>(result);

        return Ok(response);
    }

    [HttpPatch]
    public async Task<IActionResult> ResetStatistics()
    {
        var user = HttpContext.Items["User"] as UserModel;
        var result = await _questionnaireService.ResetStatistics(new Guid(user.Id));
        await _unitOfWork.SaveChangesAsync();

        var response = _mapper.Map<UserQuestionnaireDTOResponse>(result);

        return Ok(response);
    }

    [HttpPatch]
    public async Task<IActionResult> Publish()
    {
        var user = HttpContext.Items["User"] as UserModel;
        await _questionnaireService.Publish(new Guid(user.Id));
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch]
    public async Task<IActionResult> RemoveFromPublication()
    {
        var user = HttpContext.Items["User"] as UserModel;
        await _questionnaireService.RemoveFromPublication(new Guid(user.Id));
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}
