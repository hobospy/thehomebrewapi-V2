using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace thehomebrewapi.Profiles
{
    public class TastingNoteProfile : Profile
    {
        public TastingNoteProfile()
        {
            CreateMap<Entities.TastingNote, Models.TastingNoteDto>();
        }
    }
}
