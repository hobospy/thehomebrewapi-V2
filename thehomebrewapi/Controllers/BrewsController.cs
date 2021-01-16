using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using thehomebrewapi.Models;
using thehomebrewapi.Services;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Controllers
{
    [ApiController]
    [Route("api/brews")]
    public class BrewsController : ControllerBase
    {
        private readonly IHomeBrewRepository _homebrewRepository;
        private readonly IMapper _mapper;

        public BrewsController(IHomeBrewRepository homeBrewRepository, IMapper mapper)
        {
            _homebrewRepository = homeBrewRepository ??
                throw new NullArgumentException(nameof(homeBrewRepository));
            _mapper = mapper ??
                throw new NullArgumentException(nameof(mapper));
        }

        [HttpGet]
        public IActionResult GetBrews()
        {
            var brews = _homebrewRepository.GetBrews();

            return Ok(_mapper.Map<IEnumerable<BrewWithoutTastingNotesDto>>(brews));
        }

        [HttpGet("{id}", Name = "GetBrew")]
        public IActionResult GetBrew(int id, bool includeTastingNotes = false)
        {
            var brew = _homebrewRepository.GetBrew(id);

            if (brew == null)
            {
                return NotFound();
            }

            if (includeTastingNotes)
            {
                return Ok(_mapper.Map<BrewDto>(brew));
            }

            return Ok(_mapper.Map<BrewWithoutTastingNotesDto>(brew));
        }

        [HttpPost]
        public IActionResult CreateBrew([FromBody] BrewForCreationDto brew)
        {
            if (!Enum.IsDefined(typeof(EBrewedState), brew.BrewedState))
            {
                ModelState.AddModelError(
                            "Description",
                            $"The brewed state for the {brew.Name} brew must exist.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var finalBrew = _mapper.Map<Entities.Brew>(brew);

            _homebrewRepository.AddBrew(finalBrew);
            _homebrewRepository.Save();

            var createdBrewToReturn = _mapper.Map<Models.BrewDto>(finalBrew);

            return CreatedAtRoute(
                "GetBrew",
                new { id = finalBrew.ID, includeTastingNotes = true },
                createdBrewToReturn);

            throw new NotImplementedException();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBrew(int id, [FromBody] BrewForUpdateDto brew)
        {
            if (!Enum.IsDefined(typeof(EBrewedState), brew.BrewedState))
            {
                ModelState.AddModelError(
                            "Description",
                            $"The brewed state for the {brew.Name} brew must exist.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var brewEntity = _homebrewRepository.GetBrew(id, true);
            if (brewEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(brew, brewEntity);

            _homebrewRepository.UpdateBrew(brewEntity);
            _homebrewRepository.Save();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateBrew(int id,
            [FromBody] JsonPatchDocument<BrewForUpdateDto> patchDoc)
        {
            var brewEntity = _homebrewRepository.GetBrew(id, true);
            if (brewEntity == null)
            {
                return NotFound();
            }

            var brewToPatch = _mapper.Map<BrewForUpdateDto>(brewEntity);

            patchDoc.ApplyTo(brewToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Enum.IsDefined(typeof(EBrewedState), brewToPatch.BrewedState))
            {
                ModelState.AddModelError(
                            "Description",
                            $"The brewed state for the {brewToPatch.Name} brew must exist.");
            }

            if (!TryValidateModel(brewToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(brewToPatch, brewEntity);

            _homebrewRepository.UpdateBrew(brewEntity);
            _homebrewRepository.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBrew(int id)
        {
            var brewEntity = _homebrewRepository.GetBrew(id, false);
            if (brewEntity == null)
            {
                return NotFound();
            }

            _homebrewRepository.DeleteBrew(brewEntity);
            _homebrewRepository.Save();

            return NoContent();
        }
    }
}
