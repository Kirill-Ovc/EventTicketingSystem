using AutoMapper;
using EventTicketingSystem.DataAccess.Models.DTOs;
using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.DataAccess.Helpers
{
    internal class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Ticket, TicketDto>()
                .ForMember(dest => dest.EventName, opt =>
                    opt.MapFrom(src => src.EventSeat.Event.Name))
                .ForMember(dest => dest.EventDateTime, opt =>
                    opt.MapFrom(src => src.EventSeat.Event.DataAndTime))
                .ForMember(dest => dest.EventVenue, opt =>
                    opt.MapFrom(src => src.EventSeat.Event.Venue.Name))
                .ForMember(dest => dest.SeatNumber, opt =>
                    opt.MapFrom(src => src.EventSeat.Seat.Name))
                .ForMember(dest => dest.RowNumber, opt =>
                    opt.MapFrom(src => src.EventSeat.Seat.RowNumber))
                .ForMember(dest => dest.SectionName, opt =>
                    opt.MapFrom(src => src.EventSeat.Seat.Section.Name));
        }
    }
}
