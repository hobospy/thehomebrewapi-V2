using AutoMapper;
using thehomebrewapi.Helpers;

namespace thehomebrewapi.Profiles
{
    public class BrewProfile : Profile
    {
        public BrewProfile()
        {
            CreateMap<Entities.Brew, Models.BrewBasicAdditionalInfoDto>()
                .ForMember(dst => dst.RecipeDescription, opt => opt.MapFrom(src => src.Recipe.Description))
                .ForMember(dst => dst.RecipeType, opt => opt.MapFrom(src => src.Recipe.Type.ToDescriptionString()));
            CreateMap<Entities.Brew, Models.BrewDto>().ReverseMap();
            CreateMap<Entities.Brew, Models.BrewForCreationDto>().ReverseMap();
            CreateMap<Entities.Brew, Models.BrewForUpdateDto>().ReverseMap();
            CreateMap<Entities.Brew, Models.BrewFullAdditionalInfoDto>().ReverseMap();
        }
    }
}