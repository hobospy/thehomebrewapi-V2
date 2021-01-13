using AutoMapper;

namespace thehomebrewapi.Profiles
{
    public class RecipeProfile : Profile
    {
        public RecipeProfile()
        {
            CreateMap<Entities.Recipe, Models.RecipeWithoutIngredientsDto>().ReverseMap();
            CreateMap<Entities.Recipe, Models.RecipeDto>().ReverseMap();
            CreateMap<Models.RecipeForCreationDto, Models.RecipeWithoutIngredientsDto>();

        }
    }
}
