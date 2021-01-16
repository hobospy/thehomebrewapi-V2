using AutoMapper;

namespace thehomebrewapi.Profiles
{
    public class RecipeProfile : Profile
    {
        public RecipeProfile()
        {
            CreateMap<Entities.Recipe, Models.RecipeWithoutStepsDto>().ReverseMap();
            CreateMap<Entities.Recipe, Models.RecipeDto>().ReverseMap();
            CreateMap<Entities.Recipe, Models.RecipeForCreationDto>().ReverseMap();
            CreateMap<Entities.Recipe, Models.RecipeForUpdateDto>().ReverseMap();
            CreateMap<Models.RecipeForCreationDto, Models.RecipeWithoutStepsDto>();
        }
    }
}
