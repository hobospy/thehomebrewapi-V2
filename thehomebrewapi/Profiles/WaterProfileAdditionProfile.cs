using AutoMapper;

namespace thehomebrewapi.Profiles
{
    public class WaterProfileAdditionProfile : Profile
    {
        public WaterProfileAdditionProfile()
        {
            CreateMap<Entities.WaterProfileAddition, Models.WaterProfileAdditionDto>();
            CreateMap<Entities.WaterProfileAddition, Models.WaterProfileAdditionForCreationDto>().ReverseMap();
            CreateMap<Entities.WaterProfileAddition, Models.WaterProfileAdditionForUpdateDto>().ReverseMap();
        }
    }
}
