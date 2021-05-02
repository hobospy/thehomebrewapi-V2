using AutoMapper;
using thehomebrewapi.Helpers;

namespace thehomebrewapi.Profiles
{
    public class RecipeProfile : Profile
    {
        public RecipeProfile()
        {
            CreateMap<Entities.Recipe, Models.RecipeWithoutStepsDto>()
                .ForMember(src => src.Type,
                           opt => opt.MapFrom(src => src.Type.ToDescriptionString()))
                .ReverseMap();
            CreateMap<Entities.Recipe, Models.RecipeDto>()
                .ForMember(src => src.Type,
                           opt => opt.MapFrom(src => src.Type.ToDescriptionString()))
                .ReverseMap();
            CreateMap<Entities.Recipe, Models.RecipeForCreationDto>().ReverseMap();
            CreateMap<Entities.Recipe, Models.RecipeForUpdateDto>().ReverseMap();
            //CreateMap<Entities.Recipe, Models.RecipeForUpdateDto>()
            //    .ForMember(src => src.Type,
            //               opt => opt.MapFrom(src => src.Type.ToDescriptionString()));
            //CreateMap<Models.RecipeForUpdateDto, Entities.Recipe>()
            //    .ForMember(src => src.Type,
            //               opt => opt.MapFrom(src => EnumExtensions.ToEnumValueFromDescription<ETypeOfBeer>(src.Type)));

            CreateMap<Models.RecipeForCreationDto, Models.RecipeWithoutStepsDto>();
        }
    }
}
