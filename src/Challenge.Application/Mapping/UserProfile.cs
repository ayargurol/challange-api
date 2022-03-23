using AutoMapper;
using Challenge.Application.Dto.User.Request;
using Challenge.Domain;

namespace Challenge.Application.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<RegisterRequest, User>();
    }

}