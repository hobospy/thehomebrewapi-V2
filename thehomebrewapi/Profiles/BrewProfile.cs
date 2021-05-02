using AutoMapper;

namespace thehomebrewapi.Profiles
{
    public class BrewProfile : Profile
    {
        public BrewProfile()
        {
            CreateMap<Entities.Brew, Models.BrewDto>().ReverseMap();
            CreateMap<Entities.Brew, Models.BrewWithoutAdditionalInfoDto>().ReverseMap();
            CreateMap<Entities.Brew, Models.BrewForCreationDto>().ReverseMap();
            CreateMap<Entities.Brew, Models.BrewForUpdateDto>().ReverseMap();
        }
    }
}
