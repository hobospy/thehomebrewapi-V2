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

            return Ok(_mapper.Map<IEnumerable<BrewWithoutAdditionalInfoDto>>(brews));
        }

        [HttpGet("{id}", Name = "GetBrew")]
        public IActionResult GetBrew(int id, bool includeAdditionalInfo = false)
        {
            var brew = _homebrewRepository.GetBrew(id, includeAdditionalInfo);

            if (brew == null)
            {
                return NotFound();
            }

            if (includeAdditionalInfo)
            {
                return Ok(_mapper.Map<BrewDto>(brew));
            }

            return Ok(_mapper.Map<BrewWithoutAdditionalInfoDto>(brew));
        }

        [HttpPost]
        public IActionResult CreateBrew([FromBody] BrewForCreationDto brew)
        {
            if (!Enum.IsDefined(typeof(EBrewedState), brew.BrewedState))
            {
                ModelState.AddModelError(
                            "Invalid brewed state",
                            $"The brewed state [{brew.BrewedState}] for the {brew.Name} brew must exist.");
            }

            if (brew.RecipeId <= 0 || !_homebrewRepository.RecipeExists(brew.RecipeId))
            {
                ModelState.AddModelError(
                            "Recipe ID",
                            $"The associated recipe id [{brew.RecipeId}] for the {brew.Name} brew must be valid.");
            }

            var nullDateTime = new DateTime(1, 1, 1, 0, 0, 0);
            if (brew.BrewedState != 0 && brew.BrewDate == nullDateTime)
            {
                ModelState.AddModelError(
                            "Tasting note date",
                            $"The brew date [{brew.BrewDate}] for the {brew.Name} brew must be valid.");
            }

            foreach (var note in brew.TastingNotes)
            {
                if (note.Date == nullDateTime)
                {
                    var noteId = note.Note.Length > 50 ? note.Note.Substring(0, 50) + "..." : note.Note;

                    ModelState.AddModelError(
                            "Tasting note date",
                            $"The tasting note date [{note.Date}] for the {noteId} tasting note must be valid.");
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var finalBrew = _mapper.Map<Entities.Brew>(brew);

            _homebrewRepository.AddBrew(finalBrew);
            finalBrew.Recipe = _homebrewRepository.GetRecipe(brew.RecipeId, true);

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
                            "Invalid brewed state",
                            $"The brewed state [{brew.BrewedState}] for the {brew.Name} brew must exist.");
            }

            if (brew.RecipeId <= 0 || !_homebrewRepository.RecipeExists(brew.RecipeId))
            {
                ModelState.AddModelError(
                            "Recipe ID",
                            $"The associated recipe id [{brew.RecipeId}] for the {brew.Name} brew must be valid.");
            }

            var nullDateTime = new DateTime(1, 1, 1, 0, 0, 0);
            if (brew.BrewedState != 0 && brew.BrewDate == nullDateTime)
            {
                ModelState.AddModelError(
                            "Tasting note date",
                            $"The brew date [{brew.BrewDate}] for the {brew.Name} brew must be valid.");
            }

            foreach (var note in brew.TastingNotes)
            {
                if (note.Date == nullDateTime)
                {
                    var noteId = note.Note.Length > 50 ? note.Note.Substring(0, 50) + "..." : note.Note;

                    ModelState.AddModelError(
                            "Tasting note date",
                            $"The tasting note date [{note.Date}] for the {noteId} tasting note must be valid.");
                }
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
                            "Invalid brewed state",
                            $"The brewed state [{brewToPatch.BrewedState}] for the {brewToPatch.Name} brew must exist.");
            }

            if (brewToPatch.RecipeId <= 0 || !_homebrewRepository.RecipeExists(brewToPatch.RecipeId))
            {
                ModelState.AddModelError(
                            "Recipe ID",
                            $"The associated recipe id [{brewToPatch.RecipeId}] for the {brewToPatch.Name} brew must be valid.");
            }

            var nullDateTime = new DateTime(1, 1, 1, 0, 0, 0);
            if (brewToPatch.BrewedState != 0 && brewToPatch.BrewDate == nullDateTime)
            {
                ModelState.AddModelError(
                            "Tasting note date",
                            $"The brew date [{brewToPatch.BrewDate}] for the {brewToPatch.Name} brew must be valid.");
            }

            foreach (var note in brewToPatch.TastingNotes)
            {
                if (note.Date == nullDateTime)
                {
                    var noteId = note.Note.Length > 50 ? note.Note.Substring(0, 50) + "..." : note.Note;

                    ModelState.AddModelError(
                            "Tasting note date",
                            $"The tasting note date [{note.Date}] for the {noteId} tasting note must be valid.");
                }
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
