using AutoMapper;

namespace thehomebrewapi.Profiles
{
    public class RecipeStepProfile : Profile
    {
        public RecipeStepProfile()
        {
            CreateMap<Entities.RecipeStep, Models.RecipeStepDto>();
            CreateMap<Entities.RecipeStep, Models.RecipeStepWithoutIngredientsDto>();
            CreateMap<Entities.RecipeStep, Models.RecipeStepForCreationDto>().ReverseMap();
            CreateMap<Entities.RecipeStep, Models.RecipeStepForUpdateDto>().ReverseMap();
        }
    }
}
