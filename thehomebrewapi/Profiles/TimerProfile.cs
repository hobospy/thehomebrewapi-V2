using AutoMapper;

namespace thehomebrewapi.Profiles
{
    public class TimerProfile : Profile
    {
        public TimerProfile()
        {
            CreateMap<Entities.Timer, Models.TimerForCreationDto>().ReverseMap();
            CreateMap<Entities.Timer, Models.TimerDto>().ReverseMap();
        }
    }
}
