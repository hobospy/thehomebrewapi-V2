using AutoMapper;

namespace thehomebrewapi.Profiles
{
    public class WaterProfileProfile : Profile
    {
        public WaterProfileProfile()
        {
            CreateMap<Entities.WaterProfile, Models.WaterProfileDto>().ReverseMap();
            CreateMap<Entities.WaterProfile, Models.WaterProfileWithoutAdditionsDto>();
            CreateMap<Entities.WaterProfile, Models.WaterProfileForCreationDto>().ReverseMap();
            CreateMap<Entities.WaterProfile, Models.WaterProfileForUpdateDto>().ReverseMap();
        }
    }
}
