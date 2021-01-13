using AutoMapper;

namespace thehomebrewapi.Profiles
{
    public class RecipeProfile : Profile
    {
        public RecipeProfile()
        {
            CreateMap<Entities.Recipe, Models.RecipeWithoutIngredientsDto>();
            CreateMap<Entities.Recipe, Models.RecipeDto>();
        }
    }
}
