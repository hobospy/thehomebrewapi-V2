using AutoMapper;
using thehomebrewapi.Helpers;

namespace thehomebrewapi.Profiles
{
    public class WaterProfileAdditionProfile : Profile
    {
        public WaterProfileAdditionProfile()
        {
            CreateMap<Entities.WaterProfileAddition, Models.WaterProfileAdditionDto>()
                .ForMember(src => src.Unit,
                           opt => opt.MapFrom(src => src.Unit.ToDescriptionString()));
            CreateMap<Entities.WaterProfileAddition, Models.WaterProfileAdditionForCreationDto>().ReverseMap();
            CreateMap<Entities.WaterProfileAddition, Models.WaterProfileAdditionForUpdateDto>().ReverseMap();
        }
    }
}
