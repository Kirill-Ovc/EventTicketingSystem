using AutoMapper;
using EventTicketingSystem.API.Models;
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
            CreateMap<Event, EventDto>()
                .ForMember(dest => dest.VenueName, opt =>
                    opt.MapFrom(src => src.Venue.Name))
                .ForMember(dest => dest.Type, opt =>
                    opt.MapFrom(src => src.EventInfo.Type))
                .ForMember(dest => dest.PosterUrl, opt =>
                    opt.MapFrom(src => src.EventInfo.PosterUrl));
            CreateMap<EventSeat, EventSeatDto>()
                .ForMember(dest => dest.SectionId, opt =>
                    opt.MapFrom(src => src.Seat.SectionId))
                .ForMember(dest => dest.RowNumber, opt =>
                    opt.MapFrom(src => src.Seat.RowNumber))
                .ForMember(dest => dest.Prices, opt => 
                    opt.Ignore());
        }
    }
}
