using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using thehomebrewapi.Models;
using thehomebrewapi.Services;

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
        [HttpHead]
        public ActionResult<IEnumerable<BrewWithoutAdditionalInfoDto>> GetBrews()
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
        public ActionResult<BrewDto> CreateBrew([FromBody] BrewForCreationDto brew)
        {
            if (!_homebrewRepository.RecipeExists(brew.RecipeId))
            {
                ModelState.AddModelError(
                            "Recipe ID",
                            $"The associated recipe id [{brew.RecipeId}] for the {brew.Name} brew must be valid.");
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
        }

        [HttpPut("{id}")]
        public ActionResult UpdateBrew(int id, [FromBody] BrewForUpdateDto brew)
        {
            if (!_homebrewRepository.RecipeExists(brew.RecipeId))
            {
                ModelState.AddModelError(
                            "Recipe ID",
                            $"The associated recipe id [{brew.RecipeId}] for the {brew.Name} brew must be valid.");
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
        public ActionResult PartiallyUpdateBrew(int id,
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

            if (!_homebrewRepository.RecipeExists(brewToPatch.RecipeId))
            {
                ModelState.AddModelError(
                            "Recipe ID",
                            $"The associated recipe id [{brewToPatch.RecipeId}] for the {brewToPatch.Name} brew must be valid.");
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
        public ActionResult DeleteBrew(int id)
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

        [HttpOptions]
        public ActionResult GetBrewsOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST, PUT, PATCH, DELETE");
            return Ok();
        }
    }
}
