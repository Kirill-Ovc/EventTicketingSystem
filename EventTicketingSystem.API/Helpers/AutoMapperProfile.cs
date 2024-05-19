using AutoMapper;
using EventTicketingSystem.DataAccess.Models.DTOs;
using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserInfoDto>();
            CreateMap<UserInfoDto, User>()
                .ForMember(dest => dest.Id, option => option.Ignore())
                .ForMember(dest => dest.Password, option => option.Ignore());
        }
    }
}
