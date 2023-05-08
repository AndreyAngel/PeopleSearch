using AutoMapper;
using PeopleSearch.Services.Intarfaces.Models;
using PeopleSearch.Services.Intarfaces.Models;
using PeopleSearchAPI.Models.DTO;
using PeopleSearchAPI.Models.DTO.Requests;
using PeopleSearchAPI.Models.DTO.Response;
using PeopleSearchAPI.Models.DTO.Responses;

namespace IdentityAPI.Helpers;

/// <summary>
/// Class for models mapping
/// </summary>
public class MappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of <see cref="MappingProfile"/>.
    /// </summary>
    public MappingProfile()
    {
        CreateMap<RegisterDTORequest, UserModel>();

        CreateMap<UserDTORequest, UserModel>();

        CreateMap<UserModel, UserDTOResponse>();

        CreateMap<AddressDTO, AddressModel>();

        CreateMap<AddressModel, AddressDTO>();

        CreateMap<LoginDTORequest, LoginModel>();

        CreateMap<UserQuestionnaireDTORequest, UserQuestionnaireModel>();

        CreateMap<UserQuestionnaireModel, UserQuestionnaireDTOResponse>();
    }
}
