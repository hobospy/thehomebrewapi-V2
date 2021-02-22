using AutoMapper;
using thehomebrewapi.Helpers;

namespace thehomebrewapi.Profiles
{
    public class IngredientProfile : Profile
    {
        public IngredientProfile()
        {
            CreateMap<Entities.Ingredient, Models.IngredientDto>();
                //.ForMember(src => src.Unit,
                //           opt => opt.MapFrom(src => src.Unit.ToDescriptionString()));
            CreateMap<Models.IngredientForCreationDto, Entities.Ingredient>().ReverseMap();
            CreateMap<Models.IngredientForUpdateDto, Entities.Ingredient>().ReverseMap();
        }
    }
}
