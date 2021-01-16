using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace thehomebrewapi.Profiles
{
    public class BrewProfile : Profile
    {
        public BrewProfile()
        {
            CreateMap<Entities.Brew, Models.BrewDto>().ReverseMap();
            CreateMap<Entities.Brew, Models.BrewWithoutTastingNotesDto>().ReverseMap();
        }
    }
}
