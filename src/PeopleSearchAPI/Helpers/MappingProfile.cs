using AutoMapper;
using PeopleSearch.Services.Intarfaces.DTO;
using PeopleSearchAPI.Models.DTO;
using PeopleSearchAPI.Models.DTO.Requests;
using PeopleSearchAPI.Models.DTO.Response;

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
    }
}
