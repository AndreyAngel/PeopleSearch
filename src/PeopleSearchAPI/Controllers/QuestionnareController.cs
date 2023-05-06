﻿using AutoMapper;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using PeopleSearch.Domain.Core.Entities;
using PeopleSearch.Domain.Interfaces;
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
        var user = HttpContext.Items["User"] as User;
        var result = _questionnaireService.GetRecommendations(new Guid(user.Id));
        var response = _mapper.Map<UserQuestionnaireListDTOResponse>(result);

        return Ok(response);
    }

    [HttpGet("{userId:Guid}")]
    public IActionResult GetByUserId(Guid userId)
    {
        try
        {
            var result = _questionnaireService.GetById(userId);
            var response = _mapper.Map<UserQuestionnaireDTOResponse>(result);

            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserQuestionnaireDTORequest model)
    {
        var questionare = _mapper.Map<UserQuestionnaire>(model);

        var user = HttpContext.Items["User"] as User;
        questionare.Id = new Guid(user.Id);
            
        var result = await _questionnaireService.Create(questionare);
        await _unitOfWork.SaveChangesAsync();
            
        var response = _mapper.Map<UserQuestionnaireDTOResponse>(result);

        return Ok(response);
    }

    [HttpPatch]
    public async Task<IActionResult> Update(UserQuestionnaireDTORequest model)
    {
        var questionare = _mapper.Map<UserQuestionnaire>(model);
            
        var user = HttpContext.Items["User"] as User;
        questionare.Id = new Guid(user.Id);

        var result = await _questionnaireService.Update(questionare);
        await _unitOfWork.SaveChangesAsync();
            
        var response = _mapper.Map<UserQuestionnaireDTOResponse>(result);

        return Ok(response);
    }

    [HttpPatch]
    public async Task<IActionResult> ResetStatistics()
    {
        var user = HttpContext.Items["User"] as User;
        var result = _questionnaireService.ResetStatistics(new Guid(user.Id));
        await _unitOfWork.SaveChangesAsync();

        var response = _mapper.Map<UserQuestionnaireDTOResponse>(result);

        return Ok(response);
    }

    [HttpPatch]
    public async Task<IActionResult> Publish()
    {
        var user = HttpContext.Items["User"] as User;
        var result = _questionnaireService.ResetStatistics(new Guid(user.Id));

        return NoContent();
    }

    [HttpPatch]
    public async Task<IActionResult> RemoveFromPublication()
    {
        var user = HttpContext.Items["User"] as User;
        var result = _questionnaireService.ResetStatistics(new Guid(user.Id));
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}
